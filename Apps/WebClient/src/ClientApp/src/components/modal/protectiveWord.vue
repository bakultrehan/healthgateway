﻿<script lang="ts">
import Vue from "vue";
import { Component, Emit, Prop, Watch } from "vue-property-decorator";

@Component
export default class ProtectiveWordComponent extends Vue {
    @Prop() error!: boolean;
    @Prop({ default: false }) isLoading!: boolean;

    private protectiveWord = "";
    private show = false;
    private isVisible = false;

    public showModal(): void {
        this.show = true;
        if (!this.isLoading) {
            this.isVisible = true;
        }
    }

    public hideModal(): void {
        this.show = false;
        this.isVisible = false;
    }

    @Watch("isLoading")
    private onIsLoading() {
        if (!this.isLoading && this.show) {
            this.isVisible = true;
        }
    }

    @Emit()
    private submit() {
        this.show = false;
        this.isVisible = false;
        return this.protectiveWord;
    }

    @Emit()
    private cancel() {
        this.hideModal();
        return;
    }

    private handleOk(bvModalEvt: Event) {
        // Prevent modal from closing
        bvModalEvt.preventDefault();

        // Trigger submit handler
        this.handleSubmit();
    }

    private handleSubmit() {
        this.submit();

        // Hide the modal manually
        this.$nextTick(() => {
            this.hideModal();
        });
    }
}
</script>

<template>
    <b-modal
        id="protective-word-modal"
        v-model="isVisible"
        data-testid="protectiveWordModal"
        title="Restricted PharmaNet Records"
        header-bg-variant="primary"
        header-text-variant="light"
        centered
    >
        <b-row>
            <b-col>
                <form @submit.stop.prevent="handleSubmit">
                    <b-row>
                        <b-col cols="8">
                            <label for="protectiveWord-input"
                                >Protective Word
                            </label>
                            <b-form-input
                                id="protectiveWord-input"
                                v-model="protectiveWord"
                                data-testid="protectiveWordInput"
                                type="password"
                                required
                            />
                        </b-col>
                    </b-row>
                    <b-row
                        v-if="error"
                        data-testid="protectiveWordModalErrorText"
                    >
                        <b-col>
                            <span class="text-danger"
                                >Invalid protective word. Try again.</span
                            >
                        </b-col>
                    </b-row>
                </form>
            </b-col>
        </b-row>
        <template #modal-footer>
            <b-row>
                <b-col>
                    <b-row>
                        <b-col>
                            <b-button
                                data-testid="protectiveWordContinueBtn"
                                size="lg"
                                variant="primary"
                                :disabled="!protectiveWord"
                                @click="handleOk($event)"
                            >
                                Continue
                            </b-button>
                        </b-col>
                    </b-row>
                    <br />
                    <b-row data-testid="protectiveWordModalText">
                        <b-col>
                            <small>
                                Please enter the protective word required to
                                access these restricted PharmaNet records.
                            </small>
                        </b-col>
                    </b-row>
                    <b-row data-testid="protectiveWordModalMoreInfoText">
                        <b-col>
                            <small>
                                For more information visit
                                <a
                                    data-testid="protectiveWordModalRulesHREF"
                                    href="https://www2.gov.bc.ca/gov/content/health/health-drug-coverage/pharmacare-for-bc-residents/pharmanet/protective-word-for-a-pharmanet-record"
                                    >protective-word-for-a-pharmanet-record</a
                                >
                            </small>
                        </b-col>
                    </b-row>
                </b-col>
            </b-row>
        </template>
    </b-modal>
</template>

<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";
.modal-footer {
    justify-content: flex-start;
    button {
        padding: 5px 20px 5px 20px;
    }
}
</style>
