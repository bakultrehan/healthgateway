<script lang="ts">
import { IconDefinition, library } from "@fortawesome/fontawesome-svg-core";
import {
    faCheck,
    faExclamationTriangle,
} from "@fortawesome/free-solid-svg-icons";
import { Duration } from "luxon";
import Vue from "vue";
import { Component, Ref } from "vue-property-decorator";
import {
    email,
    helpers,
    minLength,
    not,
    requiredIf,
    sameAs,
} from "vuelidate/lib/validators";
import { Validation } from "vuelidate/vuelidate";
import { Action, Getter } from "vuex-class";

import LoadingComponent from "@/components/loading.vue";
import VerifySMSComponent from "@/components/modal/verifySMS.vue";
import BannerError from "@/models/bannerError";
import type { WebClientConfiguration } from "@/models/configData";
import { DateWrapper } from "@/models/dateWrapper";
import User, { OidcUserProfile } from "@/models/user";
import UserEmailInvite from "@/models/userEmailInvite";
import UserProfile from "@/models/userProfile";
import UserSMSInvite from "@/models/userSMSInvite";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import {
    IAuthenticationService,
    ILogger,
    IUserProfileService,
} from "@/services/interfaces";
import ErrorTranslator from "@/utility/errorTranslator";

library.add(faExclamationTriangle);

const userNamespace = "user";
const authNamespace = "auth";

@Component({
    components: {
        LoadingComponent,
        VerifySMSComponent,
    },
})
export default class ProfileView extends Vue {
    @Getter("oidcIsAuthenticated", {
        namespace: authNamespace,
    })
    oidcIsAuthenticated!: boolean;

    @Action("getUserEmail", { namespace: userNamespace })
    getUserEmail!: ({ hdid }: { hdid: string }) => Promise<UserEmailInvite>;

    @Action("getUserSMS", { namespace: userNamespace })
    getUserSMS!: ({ hdid }: { hdid: string }) => Promise<UserSMSInvite>;

    @Action("updateUserEmail", { namespace: userNamespace })
    updateUserEmail!: ({
        hdid,
        emailAddress,
    }: {
        hdid: string;
        emailAddress: string;
    }) => Promise<void>;

    @Action("closeUserAccount", { namespace: userNamespace })
    closeUserAccount!: ({ hdid }: { hdid: string }) => Promise<void>;

    @Action("recoverUserAccount", { namespace: userNamespace })
    recoverUserAccount!: ({ hdid }: { hdid: string }) => Promise<void>;

    @Action("updateSMSResendDateTime", { namespace: "user" })
    updateSMSResendDateTime!: ({
        hdid,
        dateTime,
    }: {
        hdid: string;
        dateTime: DateWrapper;
    }) => void;

    @Getter("user", { namespace: userNamespace }) user!: User;

    @Getter("userIsActive", { namespace: userNamespace })
    isActiveProfile!: boolean;

    @Getter("webClient", { namespace: "config" })
    webClientConfig!: WebClientConfiguration;

    @Action("addError", { namespace: "errorBanner" })
    addError!: (error: BannerError) => void;

    @Ref("verifySMSModal")
    readonly verifySMSModal!: VerifySMSComponent;

    private isLoading = true;

    private emailVerified = false;
    private email = "";
    private emailConfirmation = "";
    private isEmailEditable = false;
    private oidcUser!: OidcUserProfile;
    private emailVerificationSent = false;

    private smsVerified = false;
    private smsNumber = "";
    private isSMSEditable = false;
    private tempSMS = "";
    private invalidSMSVerificationCode = false;

    private tempEmail = "";
    private submitStatus = "";
    private logger!: ILogger;
    private userProfileService!: IUserProfileService;
    private userProfile!: UserProfile;

    private lastLoginDateString = "";

    private showCloseWarning = false;

    private timeForDeletion = -1;

    private intervalHandler = 0;

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
        this.userProfileService = container.get<IUserProfileService>(
            SERVICE_IDENTIFIER.UserProfileService
        );

        // Load the user name and current email
        let authenticationService = container.get<IAuthenticationService>(
            SERVICE_IDENTIFIER.AuthenticationService
        );

        this.isLoading = true;
        var oidcUserPromise = authenticationService.getOidcUserProfile();
        var userEmailPromise = this.getUserEmail({ hdid: this.user.hdid });
        var userSMSPromise = this.getUserSMS({ hdid: this.user.hdid });
        var userProfilePromise = this.userProfileService.getProfile(
            this.user.hdid
        );

        Promise.all([
            oidcUserPromise,
            userEmailPromise,
            userSMSPromise,
            userProfilePromise,
        ])
            .then((results) => {
                // Load oidc user details
                if (results[0]) {
                    this.oidcUser = results[0];
                }

                if (results[1]) {
                    // Load user email
                    var userEmail = results[1];
                    this.email = userEmail.emailAddress;
                    this.emailVerified = userEmail.validated;
                    this.emailVerificationSent = this.emailVerified;
                }

                if (results[2]) {
                    // Load user sms
                    var userSMS = results[2];
                    this.smsNumber = userSMS.smsNumber;
                    this.smsVerified = userSMS.validated;
                }

                if (results[3]) {
                    // Load user profile
                    this.userProfile = results[3];
                    this.logger.verbose(
                        `User Profile: ${JSON.stringify(this.userProfile)}`
                    );
                    this.lastLoginDateString = new DateWrapper(
                        this.userProfile.lastLoginDateTime
                    ).format("LLL d, yyyy");
                }

                this.isLoading = false;
            })
            .catch((err) => {
                this.logger.error(`Error loading profile: ${err}`);
                this.addError(
                    ErrorTranslator.toBannerError("Profile loading", err)
                );
                this.isLoading = false;
            });

        this.calculateTimeForDeletion();
        this.intervalHandler = window.setInterval(() => {
            this.calculateTimeForDeletion();
        }, 1000);
        this.checkToVerifyPhone();
    }

    private checkToVerifyPhone() {
        let toVerifyPhone = this.$route.query.toVerifyPhone;
        this.logger.debug(
            `toVerifyPhone: ${toVerifyPhone}; smsVerified: ${this.smsVerified}`
        );
        if (toVerifyPhone && !this.smsVerified) {
            this.logger.debug(`display Verifying SMS popup`);
            this.verifySMS();
        }
    }

    private validations() {
        const sms = helpers.regex("sms", /^\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})$/);
        return {
            smsNumber: {
                required: requiredIf(() => {
                    return this.isSMSEditable && this.smsNumber !== "";
                }),
                newSMSNumber: not(sameAs("tempSMS")),
                sms,
            },
            smsVerificationCode: {
                required: requiredIf(() => {
                    return (
                        !this.smsVerified &&
                        this.smsNumber !== "" &&
                        !this.isSMSEditable
                    );
                }),
                minLength: minLength(6),
            },
            email: {
                required: requiredIf(() => {
                    return this.isEmailEditable && this.email !== "";
                }),
                newEmail: not(sameAs("tempEmail")),
                email,
            },
            emailConfirmation: {
                required: requiredIf(() => {
                    return (
                        this.isEmailEditable && this.emailConfirmation !== ""
                    );
                }),
                sameAsEmail: sameAs("email"),
                email,
            },
        };
    }

    private get fullName(): string {
        return this.oidcUser.given_name + " " + this.oidcUser.family_name;
    }

    private calculateTimeForDeletion(): void {
        if (this.isActiveProfile) {
            return undefined;
        }

        let endDate = new DateWrapper(this.user.closedDateTime);
        endDate = endDate.add({ hour: this.webClientConfig.hoursForDeletion });
        this.timeForDeletion = endDate.diff(new DateWrapper()).milliseconds;
    }

    private get timeForDeletionString(): string {
        if (this.isActiveProfile) {
            return "";
        }

        if (this.timeForDeletion < 0) {
            return "Your account will be closed imminently";
        }

        let duration = Duration.fromMillis(this.timeForDeletion);
        let timeRemaining = duration.as("days");
        if (timeRemaining > 1) {
            return this.pluralize(timeRemaining, "day");
        }
        timeRemaining = duration.as("hours");
        if (timeRemaining > 1) {
            return this.pluralize(timeRemaining, "hour");
        }
        timeRemaining = duration.as("minutes");
        if (timeRemaining > 1) {
            return this.pluralize(timeRemaining, "minute");
        }

        timeRemaining = duration.as("seconds");
        return this.pluralize(timeRemaining, "second");
    }

    private get verifiedIcon(): IconDefinition {
        return faCheck;
    }

    private pluralize(count: number, message: string): string {
        let roundCount = Math.floor(count);
        return (
            roundCount.toString() + " " + message + (roundCount > 1 ? "s" : "")
        );
    }

    private isValid(param: Validation): boolean | undefined {
        return param.$dirty ? !param.$invalid : undefined;
    }

    private makeEmailEditable(): void {
        this.isEmailEditable = true;
        this.emailConfirmation = this.email;
        this.tempEmail = this.email || "";
    }

    private makeSMSEditable(): void {
        this.isSMSEditable = true;
        this.tempSMS = this.smsNumber || "";
    }

    private cancelEmailEdit(): void {
        this.isEmailEditable = false;
        this.email = this.tempEmail;
        this.emailConfirmation = "";
        this.tempEmail = "";
        this.$v.$reset();
    }

    private cancelSMSEdit(): void {
        this.isSMSEditable = false;
        this.smsNumber = this.tempSMS;
        this.tempSMS = "";
        this.$v.$reset();
    }

    private saveEmailEdit(): void {
        this.$v.$touch();
        if (this.$v.email.$invalid || this.$v.emailConfirmation.$invalid) {
            this.submitStatus = "ERROR";
        } else {
            this.submitStatus = "PENDING";
            this.logger.debug(`saveEmailEdit: ${JSON.stringify(this.email)}`);
            this.sendUserEmailUpdate();
        }
    }

    private saveSMSEdit(): void {
        this.$v.$touch();
        if (this.$v.smsNumber.$invalid) {
            this.submitStatus = "ERROR";
        } else {
            this.submitStatus = "PENDING";
            if (this.smsNumber) {
                this.smsNumber = this.smsNumber.replace(/\D+/g, "");
            }
            this.updateSMS();
        }
    }

    private verifySMS(): void {
        this.verifySMSModal.showModal();
    }

    private onVerifySMSSubmit(): void {
        this.getUserSMS({ hdid: this.user.hdid });
        this.smsVerified = true;
    }
    private sendUserEmailUpdate(): void {
        this.isLoading = true;
        this.updateUserEmail({
            hdid: this.user.hdid || "",
            emailAddress: this.email,
        })
            .then(() => {
                this.logger.verbose("success!");
                this.isEmailEditable = false;
                this.emailVerificationSent = true;
                this.emailConfirmation = "";
                this.tempEmail = "";
                this.getUserEmail({ hdid: this.user.hdid });
                this.$v.$reset();
            })
            .catch((err) => {
                this.logger.error(err);
                this.addError(
                    ErrorTranslator.toBannerError("Profile Update", err)
                );
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private updateSMS(): void {
        this.logger.debug(
            `Updating ${this.smsNumber ? this.smsNumber : "sms number..."}`
        );
        // Reset timer when user submits their SMS number
        this.updateSMSResendDateTime({
            hdid: this.user.hdid,
            dateTime: new DateWrapper(),
        });
        // Send update to backend
        this.userProfileService
            .updateSMSNumber(this.user.hdid, this.smsNumber)
            .then(() => {
                this.isSMSEditable = false;
                this.smsVerified = false;
                this.tempSMS = "";
                this.getUserSMS({ hdid: this.user.hdid });
                if (this.smsNumber) {
                    this.verifySMS();
                }
                this.$v.$reset();
            });
    }

    private removeEmail(): void {
        this.$v.$touch();
        this.email = "";
        this.emailConfirmation = "";
    }

    private removeSMS(): void {
        this.$v.$touch();
        this.smsNumber = "";
    }

    private recoverAccount(): void {
        this.isLoading = true;
        this.recoverUserAccount({
            hdid: this.user.hdid,
        })
            .then(() => {
                this.logger.verbose("success!");
            })
            .catch((err) => {
                this.logger.error(err);
                this.addError(
                    ErrorTranslator.toBannerError("Profile Recover", err)
                );
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private showCloseWarningBtn(): void {
        this.showCloseWarning = true;
    }

    private cancelClose(): void {
        this.showCloseWarning = false;
    }

    private closeAccount(): void {
        this.isLoading = true;
        this.closeUserAccount({
            hdid: this.user.hdid,
        })
            .then(() => {
                this.logger.verbose("success!");
                this.showCloseWarning = false;
            })
            .catch((err) => {
                this.logger.error(err);
                this.addError(
                    ErrorTranslator.toBannerError("Profile Close", err)
                );
            })
            .finally(() => {
                this.isLoading = false;
            });
    }
}
</script>

<template>
    <div class="m-3">
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <b-row class="py-1 my-3 py-md-5 fluid">
            <div class="col-12 col-lg-9 column-wrapper">
                <div id="pageTitle">
                    <h1 id="subject">Profile</h1>
                    <hr />
                </div>
                <div v-if="!isLoading">
                    <div v-if="isActiveProfile">
                        <b-row class="mb-3">
                            <b-col>
                                <label for="profileNames">Full Name</label>
                                <div id="profileNames">
                                    {{ fullName }}
                                </div>
                            </b-col>
                        </b-row>
                        <b-row class="mb-3">
                            <b-col>
                                <label for="lastLoginDate"
                                    >Last Login Date</label
                                >
                                <div id="lastLoginDate">
                                    {{ lastLoginDateString }}
                                </div>
                            </b-col>
                        </b-row>
                        <b-row class="mb-3">
                            <b-col>
                                <label for="email">Email Address</label>
                                <b-button
                                    v-if="!isEmailEditable"
                                    id="editEmail"
                                    data-testid="editEmailBtn"
                                    class="mx-auto"
                                    variant="link"
                                    @click="makeEmailEditable()"
                                >
                                    Edit
                                </b-button>
                                <b-button
                                    v-if="email"
                                    id="removeEmail"
                                    data-testid="removeEmailBtn"
                                    class="text-danger"
                                    variant="link"
                                    @click="
                                        makeEmailEditable();
                                        removeEmail();
                                    "
                                >
                                    Remove
                                </b-button>
                                <div class="form-inline">
                                    <b-form-input
                                        id="email"
                                        v-model="$v.email.$model"
                                        data-testid="emailInput"
                                        type="email"
                                        class="mb-1"
                                        :placeholder="
                                            isEmailEditable
                                                ? 'Your email address'
                                                : 'Empty'
                                        "
                                        :disabled="!isEmailEditable"
                                        :state="
                                            isValid($v.email) ||
                                            (emailVerified &&
                                            !isEmailEditable &&
                                            !!email
                                                ? true
                                                : undefined)
                                        "
                                    />
                                    <div
                                        v-if="
                                            !emailVerified &&
                                            !isEmailEditable &&
                                            email
                                        "
                                        class="ml-3"
                                    >
                                        (Not Verified)
                                    </div>
                                    <b-button
                                        v-if="
                                            !emailVerified &&
                                            !isEmailEditable &&
                                            email
                                        "
                                        id="resendEmail"
                                        data-testid="resendEmailBtn"
                                        variant="warning"
                                        class="ml-3"
                                        :disabled="emailVerificationSent"
                                        @click="sendUserEmailUpdate()"
                                    >
                                        Resend Verification
                                    </b-button>
                                </div>
                                <b-form-invalid-feedback
                                    :state="isValid($v.email)"
                                >
                                    Valid email is required
                                </b-form-invalid-feedback>
                                <b-form-invalid-feedback
                                    :state="$v.email.newEmail"
                                >
                                    New email must be different from the
                                    previous one
                                </b-form-invalid-feedback>
                            </b-col>
                        </b-row>
                        <b-row v-if="isEmailEditable" class="mb-3">
                            <b-col>
                                <b-form-input
                                    id="emailConfirmation"
                                    v-model="$v.emailConfirmation.$model"
                                    data-testid="emailConfirmationInput"
                                    type="email"
                                    placeholder="Confirm your email address"
                                    :state="isValid($v.emailConfirmation)"
                                />
                                <b-form-invalid-feedback
                                    :state="$v.emailConfirmation.sameAsEmail"
                                >
                                    Emails must match
                                </b-form-invalid-feedback>
                            </b-col>
                        </b-row>
                        <b-row v-if="!email && tempEmail">
                            <b-col
                                class="font-weight-bold text-primary text-center"
                            >
                                <font-awesome-icon
                                    icon="exclamation-triangle"
                                    aria-hidden="true"
                                ></font-awesome-icon>
                                Removing your email address will disable future
                                email communications from the Health Gateway
                            </b-col>
                        </b-row>

                        <b-row
                            v-if="isEmailEditable"
                            class="mb-3 justify-content-end"
                        >
                            <b-col class="text-right">
                                <b-button
                                    id="editEmailCancelBtn"
                                    data-testid="editEmailCancelBtn"
                                    class="mx-2 actionButton"
                                    @click="cancelEmailEdit()"
                                >
                                    Cancel
                                </b-button>
                                <b-button
                                    id="editSMSSaveBtn"
                                    data-testid="editEmailSaveBtn"
                                    variant="primary"
                                    class="mx-2 actionButton"
                                    :disabled="tempEmail === email"
                                    @click="saveEmailEdit($event)"
                                >
                                    Save
                                </b-button>
                            </b-col>
                        </b-row>
                        <b-row class="mb-3">
                            <b-col>
                                <label for="smsNumber"
                                    >Cell Number (SMS notifications)</label
                                >
                                <b-button
                                    v-if="!isSMSEditable"
                                    id="editSMS"
                                    data-testid="editSMSBtn"
                                    class="mx-auto"
                                    variant="link"
                                    @click="makeSMSEditable()"
                                    >Edit
                                </b-button>
                                <b-button
                                    v-if="smsNumber"
                                    id="removeSMS"
                                    data-testid="removeSMSBtn"
                                    class="text-danger"
                                    variant="link"
                                    @click="
                                        makeSMSEditable();
                                        removeSMS();
                                    "
                                >
                                    Remove
                                </b-button>
                                <div class="form-inline">
                                    <b-form-input
                                        id="smsNumber"
                                        v-model="$v.smsNumber.$model"
                                        v-mask="'(###) ###-####'"
                                        type="tel"
                                        data-testid="smsNumberInput"
                                        class="mb-1"
                                        :placeholder="
                                            isSMSEditable
                                                ? 'Your phone number'
                                                : 'Empty'
                                        "
                                        :disabled="!isSMSEditable"
                                        :state="
                                            isValid($v.smsNumber) ||
                                            (smsVerified &&
                                            !isSMSEditable &&
                                            !!smsNumber
                                                ? true
                                                : undefined)
                                        "
                                    />
                                    <div
                                        v-if="
                                            !smsVerified &&
                                            !isSMSEditable &&
                                            smsNumber
                                        "
                                        class="ml-3"
                                    >
                                        (Not Verified)
                                        <b-button
                                            id="verifySMS"
                                            data-testid="verifySMSBtn"
                                            variant="warning"
                                            class="ml-3"
                                            @click="verifySMS()"
                                        >
                                            Verify
                                        </b-button>
                                    </div>
                                </div>
                                <b-form-invalid-feedback
                                    :state="isValid($v.smsNumber)"
                                >
                                    Valid SMS number is required
                                </b-form-invalid-feedback>
                                <b-form-invalid-feedback
                                    :state="$v.smsNumber.newSMSNumber"
                                >
                                    New SMS number must be different from the
                                    previous one
                                </b-form-invalid-feedback>
                            </b-col>
                        </b-row>
                        <b-row v-if="!smsNumber && tempSMS">
                            <b-col
                                class="font-weight-bold text-primary text-center"
                            >
                                <font-awesome-icon
                                    icon="exclamation-triangle"
                                    aria-hidden="true"
                                ></font-awesome-icon>
                                Removing your phone number will disable future
                                SMS communications from the Health Gateway
                            </b-col>
                        </b-row>
                        <b-row
                            v-if="isSMSEditable"
                            class="mb-3 justify-content-end"
                        >
                            <b-col class="text-right">
                                <b-button
                                    id="cancelBtn"
                                    data-testid="cancelSMSEditBtn"
                                    class="mx-2 actionButton"
                                    @click="cancelSMSEdit()"
                                    >Cancel
                                </b-button>
                                <b-button
                                    id="saveBtn"
                                    data-testid="saveSMSEditBtn"
                                    variant="primary"
                                    class="mx-2 actionButton"
                                    :disabled="tempSMS === smsNumber"
                                    @click="saveSMSEdit()"
                                    >Save
                                </b-button>
                            </b-col>
                        </b-row>
                    </div>
                    <div v-else>
                        <b-row class="mb-3">
                            <b-col>
                                <font-awesome-icon
                                    icon="exclamation-triangle"
                                    aria-hidden="true"
                                    class="text-danger"
                                ></font-awesome-icon>
                                <label for="deletionWarning" class="ml-1">
                                    Account marked for removal
                                </label>
                                <div id="deletionWarning">
                                    Your account has been deactivated. If you
                                    wish to recover your account click on the
                                    "Recover Account" button before the time
                                    expires.
                                </div>
                            </b-col>
                        </b-row>
                        <b-row class="mb-3">
                            <b-col>
                                <label>Time remaining for deletion: </label>
                                {{ timeForDeletionString }}
                            </b-col>
                        </b-row>
                        <b-row class="mb-3">
                            <b-col>
                                <b-button
                                    id="recoverAccountCancelBtn"
                                    data-testid="recoverAccountCancelBtn"
                                    class="mx-auto"
                                    variant="warning"
                                    @click="recoverAccount()"
                                    >Recover Account
                                </b-button>
                            </b-col>
                        </b-row>
                    </div>
                    <b-row v-if="isActiveProfile" class="mb-3">
                        <b-col>
                            <label>Other</label>
                            <div>
                                <b-button
                                    v-if="!showCloseWarning"
                                    id="recoverAccountShowCloseWarningBtn"
                                    data-testid="recoverAccountShowCloseWarningBtn"
                                    class="p-0 pt-2"
                                    variant="link"
                                    @click="showCloseWarningBtn()"
                                    >Close My Account
                                </b-button>
                                <b-row v-if="showCloseWarning">
                                    <b-col
                                        class="font-weight-bold text-danger text-center"
                                    >
                                        <hr />
                                        <font-awesome-icon
                                            icon="exclamation-triangle"
                                            aria-hidden="true"
                                        ></font-awesome-icon>
                                        Your account will be marked for removal,
                                        preventing you from accessing your
                                        information on the Health Gateway. After
                                        a set period of time it will be removed
                                        permanently.
                                    </b-col>
                                </b-row>
                                <b-row
                                    v-if="showCloseWarning"
                                    class="mb-3 justify-content-end"
                                >
                                    <b-col class="text-right">
                                        <b-button
                                            id="closeAccountCancelBtn"
                                            data-testid="closeAccountCancelBtn"
                                            class="mx-2"
                                            @click="cancelClose()"
                                            >Cancel
                                        </b-button>
                                        <b-button
                                            id="closeAccountBtn"
                                            data-testid="closeAccountBtn"
                                            class="mx-2"
                                            variant="danger"
                                            @click="closeAccount()"
                                            >Close Account
                                        </b-button>
                                    </b-col>
                                </b-row>
                            </div>
                        </b-col>
                    </b-row>
                </div>
                <div v-else>
                    <b-row class="mb-3">
                        <b-col>
                            <content-placeholders>
                                <content-placeholders-heading />
                                <content-placeholders-text :lines="1" />
                                <content-placeholders-heading />
                                <content-placeholders-text :lines="3" />
                            </content-placeholders>
                        </b-col>
                    </b-row>
                </div>
            </div>
        </b-row>
        <VerifySMSComponent
            ref="verifySMSModal"
            :sms-number="smsNumber"
            @submit="onVerifySMSSubmit"
        />
    </div>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

#pageTitle {
    color: $primary;
}

#pageTitle hr {
    border-top: 2px solid $primary;
}

label {
    font-weight: bold;
}

input {
    width: 320px !important;
    max-width: 320px !important;
}

.actionButton {
    width: 80px;
}
</style>
