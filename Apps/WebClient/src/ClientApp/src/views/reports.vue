<script lang="ts">
import Vue from "vue";
import { Component, Ref } from "vue-property-decorator";
import { Action, Getter } from "vuex-class";

import DatePickerComponent from "@/components/datePicker.vue";
import LoadingComponent from "@/components/loading.vue";
import MessageModalComponent from "@/components/modal/genericMessage.vue";
import PageTitleComponent from "@/components/pageTitle.vue";
import COVID19ReportComponent from "@/components/report/covid19.vue";
import ImmunizationHistoryReportComponent from "@/components/report/immunizationHistory.vue";
import MedicationHistoryReportComponent from "@/components/report/medicationHistory.vue";
import MSPVisitsReportComponent from "@/components/report/mspVisits.vue";
import BannerError from "@/models/bannerError";
import type { WebClientConfiguration } from "@/models/configData";
import { DateWrapper } from "@/models/dateWrapper";
import PatientData from "@/models/patientData";
import User from "@/models/user";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import { ILogger, IPatientService } from "@/services/interfaces";
import ErrorTranslator from "@/utility/errorTranslator";

@Component({
    components: {
        LoadingComponent,
        PageTitleComponent,
        MessageModalComponent,
        MedicationHistoryReportComponent,
        MSPVisitsReportComponent,
        COVID19ReportComponent,
        ImmunizationHistoryReportComponent,
        DatePickerComponent,
    },
})
export default class ReportsView extends Vue {
    @Getter("webClient", { namespace: "config" })
    config!: WebClientConfiguration;
    @Getter("user", { namespace: "user" })
    private user!: User;

    @Ref("messageModal")
    readonly messageModal!: MessageModalComponent;
    @Ref("medicationHistoryReport")
    readonly medicationHistoryReport!: MedicationHistoryReportComponent;
    @Ref("mspVisitsReport")
    readonly mspVisitsReport!: MSPVisitsReportComponent;
    @Ref("covid19Report")
    readonly covid19Report!: COVID19ReportComponent;
    @Ref("immunizationHistoryReport")
    readonly immunizationHistoryReport!: ImmunizationHistoryReportComponent;

    @Action("addError", { namespace: "errorBanner" })
    private addError!: (error: BannerError) => void;

    private isLoading = false;
    private isGeneratingReport = false;
    private logger!: ILogger;
    private reportType = "";
    private startDate?: Date | null = null;
    private endDate?: Date | null = null;
    private reportTypeOptions = [{ value: "", text: "Select" }];
    private retryCount = 2;
    private patientData: PatientData | null = null;

    private formatDate(date: string): string {
        return new DateWrapper(date).format("yyyy-MM-dd");
    }

    private mounted() {
        this.logger = container.get<ILogger>(SERVICE_IDENTIFIER.Logger);
        if (this.config.modules["Medication"]) {
            this.reportTypeOptions.push({ value: "MED", text: "Medications" });
        }
        if (this.config.modules["Encounter"]) {
            this.reportTypeOptions.push({ value: "MSP", text: "MSP Visits" });
        }
        if (this.config.modules["Laboratory"]) {
            this.reportTypeOptions.push({
                value: "COVID-19",
                text: "COVID-19 Test Results",
            });
        }
        if (this.config.modules["Immunization"]) {
            this.reportTypeOptions.push({
                value: "Immunization",
                text: "Immunizations",
            });
        }

        this.fetchPatientData();
    }

    private clear() {
        this.reportType = "";
        this.startDate = null;
        this.endDate = null;
    }

    private showConfirmationModal() {
        this.messageModal.showModal();
    }

    private downloadPdf() {
        this.isGeneratingReport = true;
        // Fixes a possible bug with html2canvas where if the page is scrolled it could cut off the image.
        window.scrollTo(0, 0);
        let generatePromise: Promise<void>;
        switch (this.reportType) {
            case "MED":
                generatePromise = this.medicationHistoryReport.generatePdf();
                break;
            case "MSP":
                generatePromise = this.mspVisitsReport.generatePdf();
                break;
            case "COVID-19":
                generatePromise = this.covid19Report.generatePdf();
                break;
            case "Immunization":
                generatePromise = this.immunizationHistoryReport.generatePdf();
                break;
            default:
                generatePromise = Promise.resolve();
        }
        generatePromise.then(() => {
            this.isGeneratingReport = false;
        });
    }

    private fetchPatientData() {
        const patientService: IPatientService = container.get<IPatientService>(
            SERVICE_IDENTIFIER.PatientService
        );

        patientService
            .getPatientData(this.user.hdid)
            .then((result) => {
                // Load patient data
                if (result) {
                    this.patientData = new PatientData();
                    this.patientData.personalhealthnumber =
                        result.resourcePayload.personalhealthnumber;
                    this.patientData.firstname =
                        result.resourcePayload.firstname;
                    this.patientData.lastname = result.resourcePayload.lastname;
                    if (result.resourcePayload.birthdate != null) {
                        this.patientData.birthdate = this.formatDate(
                            result.resourcePayload.birthdate
                        );
                    }
                }
            })
            .catch((err) => {
                this.logger.error(`Error fetching Patient Data: ${err}`);
                if (this.retryCount > 0) {
                    this.retryCount--;
                    this.fetchPatientData();
                } else {
                    this.addError(
                        ErrorTranslator.toBannerError(
                            "Unable to retrieve patient data for generating report",
                            undefined
                        )
                    );
                }
            });
    }
}
</script>

<template>
    <div class="m-3">
        <div>
            <b-row>
                <b-col
                    id="healthInsights"
                    class="col-12 col-md-10 col-lg-9 column-wrapper"
                >
                    <PageTitleComponent :title="`Export Records`" />
                    <div class="my-3 px-3 py-4 form">
                        <b-row>
                            <b-col class="col-12 col-md-3 mb-2">
                                <b-row>
                                    <b-col>
                                        <label for="reportType">
                                            Record Type
                                        </label>
                                    </b-col>
                                </b-row>
                                <b-row>
                                    <b-col>
                                        <b-form-select
                                            id="reportType"
                                            v-model="reportType"
                                            data-testid="reportType"
                                            :options="reportTypeOptions"
                                        >
                                        </b-form-select>
                                    </b-col>
                                </b-row>
                            </b-col>
                            <b-col class="col-12 col-md-3 mb-2">
                                <b-row>
                                    <b-col>
                                        <label for="start-date">From</label>
                                    </b-col>
                                </b-row>
                                <b-row>
                                    <b-col>
                                        <DatePickerComponent
                                            id="start-date"
                                            v-model="startDate"
                                            data-testid="startDateInput"
                                        />
                                    </b-col>
                                </b-row>
                            </b-col>
                            <b-col class="col-12 col-md-6">
                                <b-row>
                                    <b-col>
                                        <label for="end-date">To</label>
                                    </b-col>
                                </b-row>
                                <b-row>
                                    <b-col class="col-12 col-md-6 mb-2">
                                        <DatePickerComponent
                                            id="end-date"
                                            v-model="endDate"
                                            data-testid="endDateInput"
                                        />
                                    </b-col>
                                    <b-col class="col-12 col-md-6">
                                        <div
                                            style="width: fit-content"
                                            class="ml-auto"
                                        >
                                            <b-button
                                                variant="secondary"
                                                data-testid="clearBtn"
                                                class="mb-1 mr-1"
                                                :disabled="isLoading"
                                                @click="clear"
                                            >
                                                Clear
                                            </b-button>
                                            <b-button
                                                variant="primary"
                                                data-testid="exportRecordBtn"
                                                class="mb-1"
                                                :disabled="
                                                    !reportType ||
                                                    isLoading ||
                                                    patientData === null
                                                "
                                                @click="showConfirmationModal"
                                            >
                                                Download PDF
                                            </b-button>
                                        </div>
                                    </b-col>
                                </b-row>
                            </b-col>
                        </b-row>
                    </div>
                    <LoadingComponent
                        v-if="isLoading || isGeneratingReport"
                        :is-loading="isLoading || isGeneratingReport"
                        :is-custom="!isGeneratingReport"
                        :backdrop="false"
                    ></LoadingComponent>
                    <div
                        v-if="reportType == 'MED'"
                        data-testid="medicationReportSample"
                        class="sample"
                    >
                        <MedicationHistoryReportComponent
                            ref="medicationHistoryReport"
                            :start-date="startDate"
                            :end-date="endDate"
                            :patient-data="patientData"
                            @on-is-loading-changed="isLoading = $event"
                        />
                    </div>
                    <div
                        v-else-if="reportType == 'MSP'"
                        data-testid="mspVisitsReportSample"
                        class="sample"
                    >
                        <MSPVisitsReportComponent
                            ref="mspVisitsReport"
                            :start-date="startDate"
                            :end-date="endDate"
                            :patient-data="patientData"
                            @on-is-loading-changed="isLoading = $event"
                        />
                    </div>
                    <div
                        v-else-if="reportType == 'COVID-19'"
                        data-testid="covid19ReportSample"
                        class="sample"
                    >
                        <COVID19ReportComponent
                            ref="covid19Report"
                            :start-date="startDate"
                            :end-date="endDate"
                            :patient-data="patientData"
                            @on-is-loading-changed="isLoading = $event"
                        />
                    </div>
                    <div
                        v-else-if="reportType == 'Immunization'"
                        data-testid="immunizationHistoryReportSample"
                        class="sample"
                    >
                        <ImmunizationHistoryReportComponent
                            ref="immunizationHistoryReport"
                            :start-date="startDate"
                            :end-date="endDate"
                            :patient-data="patientData"
                            @on-is-loading-changed="isLoading = $event"
                        />
                    </div>
                    <div v-else>
                        <b-row>
                            <b-col>
                                <img
                                    class="mx-auto d-block"
                                    src="@/assets/images/reports/reports.png"
                                    data-testid="infoImage"
                                    width="200"
                                    height="auto"
                                    alt="..."
                                />
                            </b-col>
                        </b-row>
                        <b-row>
                            <b-col class="text-center">
                                <h5 data-testid="infoText">
                                    Select a record type above to create a
                                    report
                                </h5>
                            </b-col>
                        </b-row>
                    </div>
                </b-col>
            </b-row>
        </div>
        <MessageModalComponent
            ref="messageModal"
            title="Sensitive Document Download"
            message="The file that you are downloading contains personal information. If you are on a public computer, please ensure that the file is deleted before you log off."
            @submit="downloadPdf"
        />
    </div>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.column-wrapper {
    border: 1px;
}

#pageTitle {
    color: $primary;
}

#pageTitle hr {
    border-top: 2px solid $primary;
}

.sample {
    padding: 0px 10px;
    width: 100%;
    height: 600px;
    overflow-y: scroll;
    overflow-x: hidden;
}
.form {
    background-color: $soft_background;
    border: $lightGrey solid 1px;
    border-radius: 5px 5px 5px 5px;
}
</style>
