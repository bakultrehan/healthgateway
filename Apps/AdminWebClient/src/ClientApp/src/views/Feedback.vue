<template>
    <v-container>
        <LoadingComponent :is-loading="isLoading"></LoadingComponent>
        <BannerFeedbackComponent
            :show-feedback.sync="showFeedback"
            :feedback="bannerFeedback"
            class="mt-5"
        ></BannerFeedbackComponent>
        <v-row justify="center">
            <v-col md="9">
                <v-row>
                    <v-col no-gutters>
                        <v-data-table
                            :headers="tableHeaders"
                            :items="feedbackList"
                            :items-per-page="50"
                            :footer-props="{
                                'items-per-page-options': [25, 50, 100, -1]
                            }"
                        >
                            <template #item.createdDateTime="{ item }">
                                <span>{{
                                    formatDate(item.createdDateTime)
                                }}</span>
                            </template>
                            <template #item.isReviewed="{ item }">
                                <td>
                                    <v-btn
                                        class="mx-2"
                                        dark
                                        small
                                        @click="toggleReviewed(item)"
                                    >
                                        <v-icon
                                            v-if="item.isReviewed"
                                            color="green"
                                            dark
                                            >fa-check</v-icon
                                        >
                                        <v-icon
                                            v-if="!item.isReviewed"
                                            color="red"
                                            dark
                                            >fa-times</v-icon
                                        >
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

import BannerFeedbackComponent from "@/components/core/BannerFeedback.vue";
import LoadingComponent from "@/components/core/Loading.vue";
import { ResultType } from "@/constants/resulttype";
import BannerFeedback from "@/models/bannerFeedback";
import UserFeedback from "@/models/userFeedback";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import { IUserFeedbackService } from "@/services/interfaces";

@Component({
    components: {
        LoadingComponent,
        BannerFeedbackComponent
    }
})
export default class FeedbackView extends Vue {
    private isLoading = true;
    private showFeedback = false;
    private bannerFeedback: BannerFeedback = {
        type: ResultType.NONE,
        title: "",
        message: ""
    };

    private tableHeaders = [
        {
            text: "Date",
            value: "createdDateTime",
            width: "20%"
        },
        {
            text: "Email",
            value: "email",
            width: "20%"
        },
        {
            text: "Comments",
            value: "comment",
            width: "55%"
        },
        {
            text: "Reviewed?",
            value: "isReviewed",
            width: "5%"
        }
    ];

    private feedbackList: UserFeedback[] = [];

    private userFeedbackService!: IUserFeedbackService;

    private mounted() {
        this.userFeedbackService = container.get(
            SERVICE_IDENTIFIER.UserFeedbackService
        );

        this.loadFeedbackList();
    }

    private loadFeedbackList() {
        this.userFeedbackService
            .getFeedbackList()
            .then(userFeedbacks => {
                this.feedbackList = [];
                this.feedbackList.push(...userFeedbacks);
            })
            .catch(err => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Error loading user feedbacks"
                };
                console.log(err);
            })
            .finally(() => {
                this.isLoading = false;
            });
    }

    private formatDate(date: Date): string {
        return new Date(Date.parse(date + "Z")).toLocaleString();
    }

    private toggleReviewed(feedback: UserFeedback): void {
        this.isLoading = true;
        feedback.isReviewed = !feedback.isReviewed;
        this.userFeedbackService
            .toggleReviewed(feedback)
            .then(() => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Success,
                    title: "Feedback Reviewed",
                    message: "Successfully Reviewed User Feedback."
                };
                this.loadFeedbackList();
            })
            .catch(err => {
                this.showFeedback = true;
                this.bannerFeedback = {
                    type: ResultType.Error,
                    title: "Error",
                    message: "Reviewing feedback failed, please try again."
                };
                console.log(err);
            })
            .finally(() => {
                this.isLoading = false;
            });
    }
}
</script>
