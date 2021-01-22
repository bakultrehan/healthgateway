import { DateWrapper } from "@/models/dateWrapper";
import TimelineFilter from "@/models/timelineFilter";

export const enum EntryType {
    Medication = "Medication",
    Immunization = "Immunization",
    Laboratory = "Laboratory",
    Encounter = "Encounter",
    Note = "Note",
    NONE = "NONE",
}

export class DateGroup {
    key: string;
    date: DateWrapper;
    entries: TimelineEntry[];

    constructor(key: string, date: DateWrapper, entries: TimelineEntry[]) {
        this.key = key;
        this.date = date;
        this.entries = entries;
    }

    public static createGroups(timelineEntries: TimelineEntry[]): DateGroup[] {
        if (timelineEntries.length === 0) {
            return [];
        }
        const groups = timelineEntries.reduce<Record<string, TimelineEntry[]>>(
            (groups, entry) => {
                const date = entry.date.fromEpoch();

                // Create a new group if it the date doesnt exist in the map
                if (!groups[date]) {
                    groups[date] = [];
                }
                groups[date].push(entry);
                return groups;
            },
            {}
        );
        const groupArrays = Object.keys(groups).map<DateGroup>((dateKey) => {
            return new DateGroup(
                dateKey,
                groups[dateKey][0].date,
                groups[dateKey].sort((a: TimelineEntry, b: TimelineEntry) =>
                    a.type > b.type ? 1 : a.type < b.type ? -1 : 0
                )
            );
        });
        return groupArrays;
    }

    public static sortGroup(
        groupArrays: DateGroup[],
        ascending = true
    ): DateGroup[] {
        groupArrays.sort((a, b) =>
            a.date.isAfter(b.date)
                ? -1 * (ascending ? 1 : -1)
                : a.date.isBefore(b.date)
                ? 1 * (ascending ? 1 : -1)
                : 0
        );
        return groupArrays;
    }
}

// The base timeline entry model
export default abstract class TimelineEntry {
    public readonly id: string;
    public readonly type: EntryType;
    public readonly date: DateWrapper;

    public constructor(id: string, type: EntryType, date: DateWrapper) {
        this.id = id;
        this.type = type;
        this.date = date;
    }

    public abstract keywordApplies(filter: TimelineFilter): boolean;

    public filterApplies(filter: TimelineFilter): boolean {
        return (
            this.entryTypeApplies(filter) &&
            this.dateRangeApplies(filter) &&
            this.keywordApplies(filter)
        );
    }

    private entryTypeApplies(filter: TimelineFilter): boolean {
        return (
            filter.entryTypes.every((et) => !et.isSelected) ||
            filter.entryTypes.some(
                (et) => et.type == this.type && et.isSelected
            )
        );
    }

    public dateRangeApplies(filter: TimelineFilter): boolean {
        const startDateWapper = new DateWrapper(filter.startDate);
        const endDateWapper = new DateWrapper(filter.endDate + "T23:59:59.999");
        return (
            (!filter.startDate || this.date.isAfterOrSame(startDateWapper)) &&
            (!filter.endDate || this.date.isBeforeOrSame(endDateWapper))
        );
    }
}
