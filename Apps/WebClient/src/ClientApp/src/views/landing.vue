<style lang="scss" scoped>
@import "@/assets/scss/_variables.scss";

.landing {
  .btn-secondary-landing {
    box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
    border-width: 0px;
    width: 150px;
    font-weight: 600;
    color: $primary;
    background-color: $soft_background;
  }

  .btn-primary-landing {
    box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
    border-width: 0px;
    min-width: 150px;
    font-weight: 600;
    color: $primary;
    background-color: $bcgold;
  }

  .intro {
    height: 750px;
    background-size: cover;

    /* Small Devices */
    @media (max-width: 767px) {
      /* your desktop css */
      height: 560px;
    }

    /* Very Small Devices */
    @media (max-width: 400px) {
      height: 480px;
    }

    .title-wrapper {
      margin-bottom: auto;
      margin-left: auto;
      margin-right: auto;
      max-width: 500px;

      border-radius: 25px;
      background-color: rgba(0, 0, 0, 0.5);
    }

    .logo {
      margin-bottom: 1em;
    }

    .title-text {
      color: white;
      font-size: 2.2em;
      max-width: 340px;
    }

    .register-link {
      color: white;
    }

    .register-button {
      max-width: 300px;
    }

    /* Small Devices */
    @media (max-width: 440px) {
      .title-wrapper {
        margin-top: -40px;
      }

      .title-text {
        font-size: 1.5em;
      }
    }
  }

  .devices-section {
    .devices-image {
      margin-left: auto;
      margin-right: auto;
      margin-top: -75px;
      margin-bottom: 20px;
    }

    .devices-text {
      color: $primary;
      margin-left: auto;
      margin-right: auto;
      margin-top: -40px;
      .icon-wrapper {
        /* Small Devices*/
        @media (max-width: 816px) {
          display: none;
        }
      }
      .icon-row {
        display: inline-block;
        line-height: 40px;
        height: 40px;
        vertical-align: middle;
        .icon {
          text-align: center;
          border-radius: 50%;
          width: 40px;
        }
      }

      .status-active {
        color: $primary;
        .icon {
          background-color: $primary;
          color: white;
        }
      }

      .status-inactive {
        color: darkgray;
        .icon {
          background-color: darkgray;
          color: white;
        }
      }

      .covid-container {
        color: white;
        background-color: $danger;
        border-radius: 15px;
        height: 25px;
        line-height: 28px;
        margin-top: 5px;
      }
    }
  }

  .circle-icons {
    background-color: $primary;
    color: white;
    display: flex;
    justify-content: center;

    .icon-wrapper {
      margin: 40px 35px 40px 35px;
    }

    .icon {
      color: $primary;
      height: 110px;
      width: 110px;
      padding: 25px 25px;
      margin-left: auto;
      margin-right: auto;
      margin-bottom: 10px;
    }
  }

  .tile-section {
    margin-left: 0px;
    margin-right: 0px;
    padding-left: 0px;
    padding-right: 0px;

    .col {
      padding-left: 0px;
      padding-right: 0px;
    }

    .text-wrapper {
      color: $primary;
      z-index: 1;

      .title {
        font-size: 2.2rem;
      }
    }

    /* Small Devices*/
    @media (max-width: 767px) {
      .text-wrapper {
        color: white;
        bottom: 0;
        .title {
          font-size: 2rem;
          color: white;
        }
        .small-text {
          font-size: 1.2rem;
        }
      }

      .background-tint {
        background: -moz-linear-gradient(
          top,
          rgba(255, 255, 255, 0) 0%,
          rgba(0, 0, 0, 0.8) 100%
        ); /* FF3.6+ */
        background: -webkit-gradient(
          linear,
          left top,
          left bottom,
          color-stop(0%, rgba(255, 255, 255, 0)),
          color-stop(100%, rgba(0, 0, 0, 0.65))
        ); /* Chrome,Safari4+ */
        background: -webkit-linear-gradient(
          top,
          rgba(255, 255, 255, 0) 0%,
          rgba(0, 0, 0, 0.8) 100%
        ); /* Chrome10+,Safari5.1+ */
        background: -o-linear-gradient(
          top,
          rgba(255, 255, 255, 0) 0%,
          rgba(0, 0, 0, 0.8) 100%
        ); /* Opera 11.10+ */
        background: -ms-linear-gradient(
          top,
          rgba(255, 255, 255, 0) 0%,
          rgba(0, 0, 0, 0.8) 100%
        ); /* IE10+ */
        background: linear-gradient(
          to bottom,
          rgba(255, 255, 255, 0) 0%,
          rgba(0, 0, 0, 0.8) 100%
        ); /* W3C */

        background-blend-mode: multiply;
        width: 100%;
        height: 100%;
        position: absolute;
      }
    }
  }

  .secure {
    height: 750px;
    background-size: cover;

    /* Large Devices, Wide Screens */
    @media (max-width: 767px) {
      /* your desktop css */
      height: 560px;
    }

    .secure-wrapper {
      margin-top: 130px;
      margin-bottom: auto;
    }

    .title {
      color: $primary;
      font-size: 2.2em;
    }

    .description {
      color: $primary;
    }

    .secure-buttons {
      margin-top: 1em;
    }
  }
}
</style>
<template>
  <div class="landing">
    <CommunicationComponent />
    <b-row
      class="intro align-items-center"
      :style="{ backgroundImage: 'url(\'' + introBackground + '\')' }"
    >
      <!--Empty Column for  fitting reasons-->
      <b-col class="col-12 col-md-5 col-xl-6 d-none d-md-block" />
      <b-col class="col-12 col-md-6 col-xl-4">
        <div class="title-wrapper p-4">
          <img
            class="img-fluid logo d-block mx-auto"
            :src="logo"
            width="auto"
            height="auto"
            alt="B.C. Government Logo"
          />

          <div class="title-text my-4 mx-auto">
            <div>
              Empowering you
            </div>
            <div>to manage your health</div>
          </div>
          <b-row class="">
            <b-col class="justify-content-center align align-items-center">
              <b-row>
                <b-button
                  id="btnStart"
                  to="registrationInfo"
                  class="btn btn-primary-landing mx-auto register-button w-100"
                  role="button"
                  >Register</b-button
                >
              </b-row>
              <b-row>
                <b-button
                  id="btnLogin"
                  to="login"
                  class="register-link mx-auto"
                  variant="link"
                  >Already registered? <strong>Login</strong></b-button
                >
              </b-row>
            </b-col>
          </b-row>
        </div>
      </b-col>
    </b-row>
    <b-row
      class="devices-section justify-content-center align-items-center mx-1 mx-md-5"
    >
      <b-col class="d-none d-md-block text-center col-6 col-xl-4 m-auto">
        <img
          class="img-fluid devices-image"
          :src="devices"
          width="auto"
          height="auto"
          alt="Devices"
      /></b-col>
      <b-col class="col-12 col-md-6">
        <div class="devices-text my-5 my-md-5 ml-md-4">
          <b-row>
            <h2>Browse your health records</h2>
          </b-row>
          <b-row>
            <b-col cols="1" class="icon-wrapper mr-4 px-0">
              <div
                v-for="icon in icons"
                :key="icon.label"
                class="mb-2 icon-row"
                :class="icon.active ? 'status-active' : 'status-inactive'"
                align-content="center"
              >
                <div class="icon">
                  <font-awesome-icon
                    :icon="icon.definition"
                    size="lg"
                  ></font-awesome-icon>
                </div>
              </div>
            </b-col>
            <b-col class="px-0">
              <div
                v-for="icon in icons"
                :key="icon.label"
                class="mb-2 d-flex icon-row"
                :class="icon.active ? 'status-active' : 'status-inactive'"
                align-v="center"
              >
                <b-col cols="0" class="px-0">
                  <span>{{ icon.label }}</span>
                </b-col>
                <b-col
                  v-if="icon.label === 'Lab Tests'"
                  cols="0"
                  class="covid-container ml-2 px-2"
                >
                  <font-awesome-icon
                    class="px-1"
                    icon="exclamation-triangle"
                    size="1x"
                  ></font-awesome-icon>
                  <span class="pr-1">COVID-19 Test Result</span>
                </b-col>
              </div>
            </b-col>
          </b-row>
        </div>
      </b-col>
    </b-row>
    <b-row class="circle-icons justify-content-center">
      <div
        v-for="icon in icons"
        :key="icon.label"
        class="icon-wrapper text-center px-xl-5"
      >
        <div class="icon rounded-circle bg-white">
          <h3>
            <font-awesome-icon :icon="icon.definition" size="2x" />
          </h3>
        </div>
        <span>
          {{ icon.label }}
        </span>
      </div>
    </b-row>
    <b-row class="tile-section my-0 my-md-1">
      <div>
        <b-row
          v-for="(tile, index) in tiles"
          :key="tile.title"
          class="d-flex justify-content-center align-content-around tile-row my-md-5 my-0"
        >
          <b-col class="col-12 col-md-5" :class="getTileClass(index + 1)">
            <div class="background-tint"></div>
            <img
              class="img-fluid d-md-block"
              :src="tile.imageSrc"
              width="auto"
              height="auto"
              alt="B.C. Government Logo"
          /></b-col>
          <b-col class="col-12 col-md-5" :class="getTileClass(index)">
            <div class="text-wrapper m-4 position-absolute">
              <div class="title">{{ tile.title }}</div>
              <div class="small-text">{{ tile.description }}</div>
            </div>
          </b-col>
        </b-row>
      </div>
    </b-row>
    <b-row
      class="secure justify-content-around"
      :style="{ backgroundImage: 'url(\'' + bottomImage + '\')' }"
    >
      <b-col class="col-12 col-md-6">
        <div class="secure-wrapper pl-2 pl-md-0">
          <div class="title">
            Securely access your information
          </div>
          <div class="description">
            Use your BC Services Card with two-factor authentication
          </div>
          <div class="secure-buttons">
            <b-button
              id="btnSecureStart"
              to="registrationInfo"
              class="btn btn-primary-landing m-2"
              role="button"
              >Sign Up</b-button
            >
            <b-button
              id="btnLearnMore"
              href="https://www2.gov.bc.ca/gov/content/governments/government-id/bc-services-card"
              class="btn btn-secondary-landing m-2"
              >Learn More</b-button
            >
          </div>
        </div>
      </b-col>
      <b-col class="col-0 col-md-1 d-none d-md-block" />
    </b-row>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { Component } from "vue-property-decorator";
import Image00 from "@/assets/images/landing/000_Logo-Overlay.png";
import Image01 from "@/assets/images/landing/001_Hero-01_Duotone.jpeg";
import Image02 from "@/assets/images/landing/002_Devices.png";
import Image03 from "@/assets/images/landing/003_AdobeStock_143856492-edited_shoes.jpg";
import Image04 from "@/assets/images/landing/004_AdobeStock_216356596.jpeg";
import Image05 from "@/assets/images/landing/005_AdobeStock_243861557.jpeg";
import Image06 from "@/assets/images/landing/006_AdobeStock_223963895.jpeg";
import Image07 from "@/assets/images/landing/007_Hero-02_Duotone.png";
import container from "@/plugins/inversify.config";
import CommunicationComponent from "@/components/communication.vue";
import { SERVICE_IDENTIFIER } from "@/plugins/inversify";
import {
  faPills,
  faUserMd,
  faFlask,
  faSyringe,
  IconDefinition,
  faClipboard,
  faExclamationTriangle,
} from "@fortawesome/free-solid-svg-icons";

interface Icon {
  label: string;
  definition: IconDefinition;
  active: boolean;
}

interface Tile {
  title: string;
  description: string;
  imageSrc: string;
}

@Component({
  components: {
    CommunicationComponent,
  },
})
export default class LandingComponent extends Vue {
  private icons: Icon[] = [
    {
      definition: faPills,
      label: "Medications",
      active: true,
    },
    {
      definition: faFlask,
      label: "Lab Tests",
      active: true,
    },
    {
      definition: faSyringe,
      label: "Immunizations",
      active: false,
    },
    {
      definition: faUserMd,
      label: "Consultations",
      active: false,
    },
  ];

  private tiles: Tile[] = [
    {
      title: "All in one place",
      description:
        "Conveniently access your data on a computer, tablet or smartphone",
      imageSrc: Image03,
    },
    {
      title: "Take control of your health",
      description: "Look at historical information captured over time.",
      imageSrc: Image04,
    },
    {
      title: "Manage family records",
      description:
        "Care for the needs of your children and those who depend on you.",
      imageSrc: Image05,
    },
    {
      title: "Collaborate with others",
      description:
        "Become an active participant by sharing and discussing your data with health care providers.",
      imageSrc: Image06,
    },
  ];

  private logo: string = Image00;
  private introBackground: string = Image01;
  private devices: string = Image02;
  private bottomImage: string = Image07;

  private getTileClass(index: number): string {
    return index % 2 == 0 ? "order-md-1" : "order-md-2";
  }
}
</script>