<script lang="ts">
// Load Bootstrap general plugins
import {
    AlertPlugin,
    ButtonPlugin,
    CardPlugin,
    FormCheckboxPlugin,
    FormDatepickerPlugin,
    FormGroupPlugin,
    FormInputPlugin,
    FormPlugin,
    FormRadioPlugin,
    FormRatingPlugin,
    FormSelectPlugin,
    FormTextareaPlugin,
    IconsPlugin,
    InputGroupPlugin,
    LayoutPlugin,
    LinkPlugin,
    ModalPlugin,
    NavbarPlugin,
    NavPlugin,
    PaginationNavPlugin,
    SpinnerPlugin,
    TooltipPlugin,
} from "bootstrap-vue";

Vue.use(LayoutPlugin);
Vue.use(NavPlugin);
Vue.use(NavbarPlugin);
Vue.use(ModalPlugin);
Vue.use(ButtonPlugin);
Vue.use(CardPlugin);
Vue.use(LinkPlugin);
Vue.use(FormPlugin);
Vue.use(FormInputPlugin);
Vue.use(FormCheckboxPlugin);
Vue.use(FormTextareaPlugin);
Vue.use(AlertPlugin);
Vue.use(SpinnerPlugin);
Vue.use(InputGroupPlugin);
Vue.use(PaginationNavPlugin);
Vue.use(TooltipPlugin);
Vue.use(FormGroupPlugin);
Vue.use(FormRadioPlugin);
Vue.use(FormRatingPlugin);
Vue.use(FormSelectPlugin);
Vue.use(FormDatepickerPlugin);
Vue.use(IconsPlugin);
Vue.use(VueTheMask);

// Load general icons
import { config, library } from "@fortawesome/fontawesome-svg-core";
// Prevent auto adding CSS to the header since that breaks Content security policies.
config.autoAddCss = false;
// Add font awesome styles manually
import "@fortawesome/fontawesome-svg-core/styles.css";

import {
    faAddressCard,
    faAngleDoubleLeft,
    faChartBar,
    faChartLine,
    faCheckCircle,
    faChevronDown,
    faChevronLeft,
    faChevronRight,
    faChevronUp,
    faClipboardList,
    faCommentAlt,
    faEdit,
    faEllipsisV,
    faExclamationTriangle,
    faFileAlt,
    faFlask,
    faLock,
    faPills,
    faPrint,
    faSpinner,
    faStream,
    faSyringe,
    faTimes,
    faTimesCircle,
    faUser,
    faUserCircle,
    faUserFriends,
    faUserMd,
    faUserPlus,
    faUserSecret,
} from "@fortawesome/free-solid-svg-icons";
import Vue from "vue";
import { Component, Ref, Watch } from "vue-property-decorator";
import VueTheMask from "vue-the-mask";
import { Getter } from "vuex-class";

import Process, { EnvironmentType } from "@/constants/process.ts";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import container from "@/plugins/inversify.config";
import { ILogger } from "@/services/interfaces";
library.add(
    faUser,
    faUserCircle,
    faAddressCard,
    faUserSecret,
    faEdit,
    faChevronUp,
    faChevronDown,
    faChevronLeft,
    faChevronRight,
    faSpinner,
    faCheckCircle,
    faTimes,
    faTimesCircle,
    faEllipsisV,
    faPrint,
    faAngleDoubleLeft,
    faFileAlt,
    faChartBar,
    faCommentAlt,
    faLock,
    faExclamationTriangle,
    faFlask,
    faPills,
    faSyringe,
    faUserMd,
    faClipboardList,
    faChartLine,
    faStream,
    faUserPlus,
    faUserFriends
);

import ErrorCard from "@/components/errorCard.vue";
import IdleComponent from "@/components/modal/idle.vue";
import FooterComponent from "@/components/navmenu/navFooter.vue";
import HeaderComponent from "@/components/navmenu/navHeader.vue";
import SidebarComponent from "@/components/navmenu/sidebar.vue";

const logger: ILogger = container.get(SERVICE_IDENTIFIER.Logger);

@Component({
    components: {
        NavHeader: HeaderComponent,
        NavFooter: FooterComponent,
        NavSidebar: SidebarComponent,
        ErrorCard: ErrorCard,
        IdleComponent,
    },
})
export default class App extends Vue {
    @Ref("idleModal") readonly idleModal?: IdleComponent;
    @Getter("oidcIsAuthenticated", { namespace: "auth" })
    oidcIsAuthenticated?: boolean;

    private readonly host: string = window.location.hostname.toLocaleUpperCase();
    private readonly isProduction: boolean =
        Process.NODE_ENV == EnvironmentType.production &&
        (this.host.startsWith("HEALTHGATEWAY") ||
            this.host.startsWith("WWW.HEALTHGATEWAY"));

    constructor() {
        super();
        logger.debug(
            `Node ENV: ${JSON.stringify(
                Process.NODE_ENV
            )}; host: ${JSON.stringify(this.host)}`
        );
    }

    @Watch("isAppIdle")
    private onIsAppIdleChanged(idle: boolean) {
        if (idle && this.oidcIsAuthenticated) {
            this.idleModal?.show();
        }
    }
}
</script>

<template>
    <div id="app-root" class="container-fluid-fill d-flex h-100 flex-column">
        <div v-if="!isProduction" class="devBanner">
            <div class="text-center bg-warning small">
                Non-production environment:
                <b>{{ host }}</b>
            </div>
        </div>

        <NavHeader />
        <b-row>
            <NavSidebar class="no-print sticky-top vh-100" />
            <main class="col fill-height">
                <ErrorCard
                    title="Whoops!"
                    description="An error occurred."
                    show="true"
                />
                <router-view></router-view>
                <IdleComponent ref="idleModal" />
            </main>
        </b-row>

        <footer class="footer no-print">
            <NavFooter />
        </footer>
    </div>
</template>

<style lang="scss" scoped>
.row {
    margin: 0px;
    padding: 0px;
}

.col {
    margin: 0px;
    padding: 0px;
}
</style>
<style lang="scss">
@import "@/assets/scss/_variables.scss";
@media print {
    .navbar {
        display: flex !important;
    }

    .no-print,
    .no-print * {
        display: none !important;
    }
}

html {
    height: 100vh;
}

body {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
}

main {
    padding-bottom: 0px;
    padding-top: 0px;
}

#app-root {
    min-height: 100vh;
}

.fill-height {
    margin: 0px;
    min-height: 100vh;
}

.devBanner {
    z-index: $z_top_layer;
}
</style>
