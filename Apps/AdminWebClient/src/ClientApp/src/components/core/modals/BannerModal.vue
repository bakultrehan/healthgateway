<template>
    <v-dialog v-model="dialog" persistent max-width="1000px">
        <template #activator="{ on, attrs }">
            <v-btn color="primary" dark v-bind="attrs" v-on="on"
                >New Banner Communication</v-btn
            >
        </template>
        <v-card dark>
            <v-card-title>
                <span class="headline">{{ formTitle }}</span>
            </v-card-title>
            <v-card-text>
                <v-form ref="form" lazy-validation>
                    <v-row>
                        <v-col>
                            <ValidationProvider
                                v-slot="{
                                    errors
                                }"
                                :rules="
                                    dateTimeRules(
                                        editedItem.effectiveDateTime,
                                        editedItem.expiryDateTime
                                    )
                                "
                            >
                                <v-datetime-picker
                                    v-model="editedItem.effectiveDateTime"
                                    requried
                                    label="Effective On"
                                ></v-datetime-picker>
                                <span class="error-message">{{
                                    errors[0]
                                }}</span>
                            </ValidationProvider>
                        </v-col>
                        <v-col>
                            <ValidationProvider
                                v-slot="{
                                    errors
                                }"
                                :rules="
                                    dateTimeRules(
                                        editedItem.effectiveDateTime,
                                        editedItem.expiryDateTime
                                    )
                                "
                            >
                                <v-datetime-picker
                                    v-model="editedItem.expiryDateTime"
                                    required
                                    label="Expires On"
                                ></v-datetime-picker>
                                <span class="error-message">{{
                                    errors[0]
                                }}</span>
                            </ValidationProvider>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <v-text-field
                                v-model="editedItem.subject"
                                label="Subject"
                                maxlength="100"
                                :rules="[v => !!v || 'Subject is required']"
                                validate-on-blur
                                required
                            ></v-text-field>
                        </v-col>
                        <v-col>
                            <v-text-field
                                v-model="editedItem.communicationStatusCode"
                                label="Status"
                                disabled
                            ></v-text-field>
                        </v-col>
                        <v-col>
                            <v-btn
                                color="blue darken-1"
                                text
                                @click="togglePublish()"
                                >{{ publishingStatus }}</v-btn
                            >
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <TiptapVuetify
                                v-model="editedItem.text"
                                :toolbar-attributes="{ color: 'gray' }"
                                placeholder="Write the banner content here..."
                                :extensions="extensions"
                            />
                        </v-col>
                    </v-row>
                </v-form>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" text @click="close()"
                    >Cancel</v-btn
                >
                <v-btn color="blue darken-1" text @click="save()">Save</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>
<script lang="ts">
import moment from "moment";
import {
    Blockquote,
    Bold,
    BulletList,
    Code,
    HardBreak,
    Heading,
    History,
    Italic,
    Link,
    ListItem,
    OrderedList,
    Paragraph,
    Strike,
    TiptapVuetify,
    Underline
} from "tiptap-vuetify";
import { extend, ValidationProvider } from "vee-validate";
import { Component, Emit, Prop, Vue, Watch } from "vue-property-decorator";

import Communication, {
    CommunicationStatus
} from "@/models/adminCommunication";

extend("dateValid", {
    validate(value: unknown, args: unknown[] | Record<string, Date>) {
        // We know is a record
        args = args as Record<string, Date>;
        if (moment(args.effective).isBefore(moment(args.expiry))) {
            return true;
        }
        return "Effective date must occur before expiry date.";
    },
    params: ["effective", "expiry"]
});

@Component({
    components: {
        ValidationProvider,
        TiptapVuetify
    }
})
export default class BannerModal extends Vue {
    private dialog = false;
    private extensions = [
        History,
        Blockquote,
        Link,
        Underline,
        Strike,
        Bold,
        Italic,
        ListItem,
        BulletList,
        OrderedList,
        [
            Heading,
            {
                options: {
                    levels: [1, 2, 3, 4]
                }
            }
        ],
        Bold,
        Code,
        Paragraph,
        HardBreak
    ];

    @Prop() editedItem!: Communication;
    @Prop() isNew!: number;

    private get isDraft(): boolean {
        return (
            this.editedItem.communicationStatusCode ===
            CommunicationStatus.Draft
        );
    }

    private get publishingStatus(): string {
        return this.isDraft ? "Publish" : "Draft";
    }

    @Watch("editedItem")
    private onPropChange() {
        if (!this.isNew) {
            this.dialog = true;
        }
    }

    private get formTitle(): string {
        return this.isNew ? "New Banner Post" : "Edit Banner Post";
    }

    private dateTimeRules(effective: Date, expiry: Date) {
        return "dateValid:" + effective.toString() + "," + expiry.toString();
    }

    private dateTimeValid(): boolean {
        return moment(this.editedItem.effectiveDateTime).isBefore(
            moment(this.editedItem.expiryDateTime)
        );
    }

    @Watch("dialog")
    private onDialogChange(dialogIsOpen: boolean) {
        dialogIsOpen || this.close();
    }

    private save() {
        this.editedItem.scheduledDateTime = new Date();
        if (
            (this.$refs.form as Vue & { validate: () => boolean }).validate() &&
            this.dateTimeValid()
        ) {
            if (!this.isNew) {
                this.emitUpdate(this.editedItem);
            } else {
                this.emitAdd(this.editedItem);
            }
            this.close();
        }
    }

    private togglePublish() {
        if (this.isDraft) {
            this.editedItem.communicationStatusCode = CommunicationStatus.New;
        } else {
            this.editedItem.communicationStatusCode = CommunicationStatus.Draft;
        }
        this.save();
    }

    private close() {
        this.$nextTick(() => {
            (this.$refs.form as Vue & {
                resetValidation: () => void;
            }).resetValidation();
            this.dialog = false;
            this.emitClose();
        });
    }

    @Emit()
    private emitAdd(communication: Communication) {
        return communication;
    }

    @Emit()
    private emitUpdate(communication: Communication) {
        return communication;
    }

    @Emit()
    private emitClose() {
        return;
    }
}
</script>
