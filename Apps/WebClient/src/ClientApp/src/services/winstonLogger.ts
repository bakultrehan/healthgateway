import { Logger, createLogger, format, transports } from "winston";
import { ILogger } from "@/services/interfaces";
import { injectable } from "inversify";
import { ExternalConfiguration } from "@/models/configData";

@injectable()
export class WinstonLogger implements ILogger {
    private logger!: Logger;

    public initialize(config: ExternalConfiguration): void {
        this.logger = createLogger({
            level:
                config.webClient.logLevel !== undefined
                    ? config.webClient.logLevel
                    : "warning",
            format: format.json(),
            transports: [
                new transports.Console({
                    format: format.simple(),
                }),
            ],
        });
    }

    public log(level: string, message: string): void {
        this.logger.log({
            level: level,
            message: message,
        });
    }
    public error(message: string): void {
        this.log("error", message);
    }
    public warn(message: string): void {
        this.log("warn", message);
    }
    public info(message: string): void {
        this.log("info", message);
    }
    public verbose(message: string): void {
        this.log("verbose", message);
    }
    public debug(message: string): void {
        this.log("debug", message);
    }
}
