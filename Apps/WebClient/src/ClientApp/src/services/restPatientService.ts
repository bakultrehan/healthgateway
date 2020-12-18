import { injectable } from "inversify";
import { IHttpDelegate, IPatientService } from "@/services/interfaces";
import RequestResult from "@/models/requestResult";
import PatientData from "@/models/patientData";
import { ExternalConfiguration } from "@/models/configData";

@injectable()
export class RestPatientService implements IPatientService {
    private readonly PATIENT_BASE_URI: string = "v1/api/Patient";
    private baseUri = "";
    private http!: IHttpDelegate;

    public initialize(
        config: ExternalConfiguration,
        http: IHttpDelegate
    ): void {
        this.baseUri = config.serviceEndpoints["Patient"];
        this.http = http;
    }

    public getPatientData(hdid: string): Promise<RequestResult<PatientData>> {
        return this.http.get<RequestResult<PatientData>>(
            `${this.baseUri}${this.PATIENT_BASE_URI}/${hdid}`
        );
    }
}
