import { Dictionary } from "vue-router/types/router";
import AuthenticationData from "@/models/authenticationData";
import UserBetaRequest from "@/models/userBetaRequest";
import ExternalConfiguration from "@/models/externalConfiguration";

export interface IConfigService {
  initialize(http: IHttpDelegate): void;
  getConfiguration(): Promise<ExternalConfiguration>;
}

export interface IAuthenticationService {
  initialize(http: IHttpDelegate): void;
  startLoginFlow(redirectUri: string): void;
  getAuthentication(): Promise<AuthenticationData>;
  refreshToken(): Promise<AuthenticationData>;
  destroyToken(): Promise<void>;
}

export interface IBetaRequestService {
  initialize(http: IHttpDelegate): void;
  getPendingRequests(): Promise<UserBetaRequest[]>;
  sendBetaInvites(requestsIds: string[]): Promise<string[]>;
}

export interface IHttpDelegate {
  unsetAuthorizationHeader(): void;
  setAuthorizationHeader(accessToken: string): void;
  getWithCors<T>(url: string, headers?: Dictionary<string>): Promise<T>;
  get<T>(url: string, headers?: Dictionary<string>): Promise<T>;
  post<T>(url: string, payload: Object): Promise<T>;
  put<T>(
    url: string,
    payload: Object,
    headers?: Dictionary<string>
  ): Promise<T>;
  patch<T>(
    url: string,
    payload: Object,
    headers?: Dictionary<string>
  ): Promise<T>;
}
