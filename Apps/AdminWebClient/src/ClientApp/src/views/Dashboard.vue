<script lang="ts">
import { DateTime } from "luxon";
import { Component, Vue, Watch } from "vue-property-decorator";

import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import { IDashboardService } from "@/services/interfaces";

interface DailyData {
    date: DateTime;
    registered?: number;
    loggedIn?: number;
    dependents?: number;
}

@Component
export default class Dashboard extends Vue {
    private totalRegisteredUserCount = 0;
    private totalDependentCount = 0;

    private isLoadingRegistered = true;
    private isLoadingLoggedIn = true;
    private isLoadingDependent = true;

    private datePickerModal = false;

    private today = DateTime.local();

    private dashboardService!: IDashboardService;

    private selectedDates: string[] = [
        DateTime.local()
            .minus({ days: 10 })
            .toISO()
            .substr(0, 10),
        DateTime.local()
            .toISO()
            .substr(0, 10)
    ];

    private tableData: DailyData[] = [];

    @Watch("isLoading")
    private onIsLoading(newVal: boolean, oldVal: boolean) {
        if (oldVal && !newVal) {
            this.tableData.sort((a, b) => {
                return a.date > b.date ? 1 : a.date < b.date ? -1 : 0;
            });
        }
    }

    private get visibleTableData(): DailyData[] {
        let visible: DailyData[] = [];
        let startDate = DateTime.fromISO(this.selectedDates[0]);
        let endDate = DateTime.fromISO(this.selectedDates[1]);
        this.tableData.forEach(element => {
            if (startDate <= element.date && element.date <= endDate) {
                visible.push(element);
            }
        });
        return visible;
    }

    private get isLoading(): boolean {
        return (
            this.isLoadingRegistered ||
            this.isLoadingLoggedIn ||
            this.isLoadingDependent
        );
    }

    private tableHeaders = [
        {
            text: "Date",
            value: "date"
        },
        { text: "Registered", value: "registered" },
        { text: "Logged In", value: "loggedIn" },
        { text: "Dependents", value: "dependents" }
    ];

    private mounted() {
        this.dashboardService = container.get(
            SERVICE_IDENTIFIER.DashboardService
        );
        this.getRegisteredUserCount();
        this.getLoggedInUsersCount();
        this.getDependentCount();
    }

    private getRegisteredUserCount() {
        this.isLoadingRegistered = true;
        this.dashboardService
            .getRegisteredUsersCount()
            .then(count => {
                for (let key in count) {
                    var countDate = DateTime.fromISO(key);
                    var dateValue = count[key];
                    var dayData = this.tableData.find(
                        x => x.date.toMillis() === countDate.toMillis()
                    );
                    if (dayData !== undefined) {
                        dayData.registered = dateValue;
                    } else {
                        this.tableData.push({
                            date: countDate,
                            registered: dateValue
                        });
                    }
                    this.totalRegisteredUserCount += dateValue;
                }
            })
            .finally(() => {
                this.isLoadingRegistered = false;
            });
    }

    private getLoggedInUsersCount() {
        this.isLoadingLoggedIn = true;
        this.dashboardService
            .getLoggedInUsersCount()
            .then(count => {
                for (let key in count) {
                    var countDate = DateTime.fromISO(key);
                    var dateValue = count[key];
                    var dayData = this.tableData.find(
                        x => x.date.toMillis() === countDate.toMillis()
                    );
                    if (dayData !== undefined) {
                        dayData.loggedIn = dateValue;
                    } else {
                        this.tableData.push({
                            date: countDate,
                            loggedIn: dateValue
                        });
                    }
                }
            })
            .finally(() => {
                this.isLoadingLoggedIn = false;
            });
    }

    private getDependentCount() {
        this.isLoadingDependent = true;
        this.dashboardService
            .getDependentCount()
            .then(count => {
                for (let key in count) {
                    var countDate = DateTime.fromISO(key);
                    var dateValue = count[key];
                    var dayData = this.tableData.find(
                        x => x.date.toMillis() === countDate.toMillis()
                    );
                    if (dayData !== undefined) {
                        dayData.dependents = dateValue;
                    } else {
                        this.tableData.push({
                            date: countDate,
                            dependents: dateValue
                        });
                    }

                    this.totalDependentCount += dateValue;
                }
            })
            .finally(() => {
                this.isLoadingDependent = false;
            });
    }

    private formatDate(date: DateTime): string {
        return date.toFormat("dd/MM/yyyy");
    }
}
</script>

<template>
    <v-container>
        <h2>Totals</h2>
        <v-row v-if="!isLoading" class="px-2">
            <v-col class="col-lg-3 col-md-6 col-sm-12">
                <v-card class="text-center">
                    <h3>Registered Users</h3>
                    <h1>
                        {{ totalRegisteredUserCount }}
                    </h1>
                </v-card>
            </v-col>
            <v-col class="col-lg-3 col-md-6 col-sm-12">
                <v-card class="text-center">
                    <h3>Dependents</h3>
                    <h1>
                        {{ totalDependentCount }}
                    </h1>
                </v-card>
            </v-col>
        </v-row>
        <v-row v-else>
            <v-col>
                <v-skeleton-loader
                    max-width="200"
                    type="card"
                ></v-skeleton-loader></v-col
        ></v-row>
        <br />
        <h2>Daily Data</h2>
        <v-row>
            <v-col cols="12" sm="6" md="4">
                <v-dialog
                    ref="dialog"
                    v-model="datePickerModal"
                    :return-value.sync="selectedDates"
                    persistent
                    :disabled="isLoading"
                    width="290px"
                >
                    <template #activator="{ on, attrs }">
                        <v-row>
                            <v-col>
                                <v-text-field
                                    v-model="selectedDates[0]"
                                    label="Start Date"
                                    prepend-icon="mdi-calendar"
                                    readonly
                                    v-bind="attrs"
                                    v-on="on"
                                ></v-text-field>
                            </v-col>
                            <v-col>
                                <v-text-field
                                    v-model="selectedDates[1]"
                                    label="End Date"
                                    readonly
                                    v-bind="attrs"
                                    v-on="on"
                                ></v-text-field>
                            </v-col>
                        </v-row>
                    </template>
                    <v-date-picker
                        v-model="selectedDates"
                        :max="today.toISO()"
                        min="2019-06-01"
                        range
                        scrollable
                        no-title
                    >
                        <v-spacer></v-spacer>
                        <v-btn text color="primary" @click="modal = false">
                            Cancel
                        </v-btn>
                        <v-btn
                            text
                            color="primary"
                            @click="$refs.dialog.save(selectedDates)"
                        >
                            OK
                        </v-btn>
                    </v-date-picker>
                </v-dialog>
            </v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-data-table
                    :headers="tableHeaders"
                    :items="visibleTableData"
                    :items-per-page="30"
                    :loading="isLoading"
                    loading-text="Loading... Please wait"
                    :footer-props="{
                        itemsPerPageOptions: [30, 60, 90, -1]
                    }"
                >
                    <template #[`item.date`]="{ item }">
                        <span>{{ formatDate(item.date) }}</span>
                    </template>
                </v-data-table>
            </v-col>
        </v-row>
    </v-container>
</template>

<style scoped></style>
