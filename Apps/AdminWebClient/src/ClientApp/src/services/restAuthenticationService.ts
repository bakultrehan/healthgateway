import { IAuthenticationService, IHttpDelegate } from "@/services/interfaces";
import { injectable } from "inversify";
import "reflect-metadata";
import AuthenticationData from "@/models/authenticationData";

@injectable()
export class RestAuthenticationService implements IAuthenticationService {
  private readonly AUTH_BASE_URI: string = "v1/api/Authentication";
  private http!: IHttpDelegate;

  public initialize(http: IHttpDelegate): void {
    this.http = http;
  }

  public startLoginFlow(relativeToPath: string): void {
    // Handle OIDC login by setting a hint that the AuthServer needs to know which IdP to route to
    // The server-side backend keeps the client secret needed to route to KeyCloak AS
    // We get back a JWT signed if the authentication was successful
    console.log("Starting login flow....");

    var fullRedirectUrl = new URL(relativeToPath, window.location.href);

    var authPathUrl = new URL(
      this.AUTH_BASE_URI + "/Login",
      window.location.href
    );

    let queryParams = `?redirectUri=${fullRedirectUrl.href}`;
    let fullPath = authPathUrl + queryParams;

    console.log(fullPath);
    window.location.href = fullPath;
  }

  public getAuthentication(): Promise<AuthenticationData> {
    return new Promise((resolve, reject) => {
      this.http
        .getWithCors<AuthenticationData>(`${this.AUTH_BASE_URI}/`)
        .then(result => {
          return resolve(result);
        })
        .catch(err => {
          console.log("Fetch error:" + err.toString());
          reject(err);
        });
    });
  }

  public refreshToken(): Promise<AuthenticationData> {
    return new Promise((resolve, reject) => {
      // NOT IMPLEMENTED
      resolve();
    });
  }

  public destroyToken(): Promise<void> {
    return new Promise((resolve, reject) => {
      console.log("Starting Logout flow....");
      var authPathUrl = new URL("/Auth/Logout", window.location.href);
      window.location.href = authPathUrl.href;
      this.expireSiteMinderCookie();
    });
  }

  public expireSiteMinderCookie() {
    // This expires the siteminder cookie preventing the app from login in using the cache.
    var d = new Date();
    document.cookie = `SMSESSION=;domain=.gov.bc.ca;path=/;expires=${d.toUTCString()}`;
  }
}
