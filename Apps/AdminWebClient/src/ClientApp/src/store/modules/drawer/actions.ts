import { ActionTree } from "vuex";

import { DrawerState, RootState } from "@/models/storeState";

export const actions: ActionTree<DrawerState, RootState> = {
    initialize(): boolean {
        console.log("Initializing the config store...");
        return true;
    },

    setState(context, params: { isDrawerOpen: boolean }): void {
        context.commit("setDrawerState", params.isDrawerOpen);
    }
};
