import Axios, { AxiosRequestConfig } from "axios";
import { IHttpDelegate } from "./interfaces";
import { injectable } from "inversify";
import { Dictionary } from "vue-router/types/router";

@injectable()
export default class HttpDelegate implements IHttpDelegate {
  public unsetAuthorizationHeader(): void {
    Axios.defaults.headers.common = {};
  }

  public setAuthorizationHeader(accessToken: string): void {
    Axios.defaults.headers.common = {
      Authorization: `Bearer ${accessToken}`
    };
  }

  public getWithCors<T>(
    url: string,
    headers: Dictionary<string> = {}
  ): Promise<T> {
    headers["Access-Control-Allow-Origin"] = "*";
    return this.get(url, headers);
  }

  public get<T>(
    url: string,
    headers: Dictionary<string> | undefined = undefined
  ): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      let config: AxiosRequestConfig = {
        headers
      };
      console.log(headers);
      Axios.get(url, config)
        .then(response => {
          if (response.data instanceof Object) {
            return resolve(response.data);
          } else {
            console.log(response);
            return reject("invalid request");
          }
        })
        .catch(err => {
          const errorMessage: string = `Fetch error: ${err.toString()}`;
          console.log(errorMessage);
          return reject(errorMessage);
        });
    });
  }
}
