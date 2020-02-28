﻿//-------------------------------------------------------------------------
// Copyright © 2020 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
// <auto-generated />
#pragma warning disable CS1591
namespace HealthGateway.Database.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AccountClosureTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "gateway",
                table: "EmailTemplate",
                columns: new[] { "EmailTemplateId", "Body", "CreatedBy", "CreatedDateTime", "EffectiveDate", "ExpiryDate", "FormatCode", "From", "Name", "Priority", "Subject", "UpdatedBy", "UpdatedDateTime" },
                values: new object[,]
                {
                    { new Guid("79503a38-c14a-4992-b2fe-5586629f552e"), @"<!doctype html>
                <html lang=""en"">
                <head>
                </head>
                <body style=""margin:0"">
                    <table cellspacing=""0"" align=""left"" width=""100%"" style=""margin:0;color:#707070;font-family:Helvetica;font-size:12px;"">
                        <tr style=""background:#036;"">
                            <th width=""45""></th>
                            <th width=""350"" align=""left"" style=""text-align:left;"">
                                <div role=""img"" aria-label=""Health Gateway Logo"">
                                    <img src=""${host}/Logo.png"" alt=""Health Gateway Logo"" />
                                </div>
                            </th>
                            <th width=""""></th>
                        </tr>
                        <tr>
                            <td colspan=""3"" height=""20""></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <h1 style=""font-size:18px;"">Hi,</h1>
                                <p>
                                    You have closed your Health Gateway account. If you would like to recover your account, please login to Health Gateway within the next 30 days and click “Recover Account”. No further action is required if you want your account and personally entered information to be removed from the Health Gateway after this time period.
                                </p>
                                <p>Thanks,</p>
                                <p>Health Gateway Team</p>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </body>
                </html>", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HTML", "HG_Donotreply@gov.bc.ca", "AccountClosed", 10, "Health Gateway Account Closed ", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("2fe8c825-d4de-4884-be6a-01a97b466425"), @"<!doctype html>
                <html lang=""en"">
                <head>
                </head>
                <body style=""margin:0"">
                    <table cellspacing=""0"" align=""left"" width=""100%"" style=""margin:0;color:#707070;font-family:Helvetica;font-size:12px;"">
                        <tr style=""background:#036;"">
                            <th width=""45""></th>
                            <th width=""350"" align=""left"" style=""text-align:left;"">
                                <div role=""img"" aria-label=""Health Gateway Logo"">
                                    <img src=""${host}/Logo.png"" alt=""Health Gateway Logo"" />
                                </div>
                            </th>
                            <th width=""""></th>
                        </tr>
                        <tr>
                            <td colspan=""3"" height=""20""></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <h1 style=""font-size:18px;"">Hi,</h1>
                                <p>
                                    You have successfully recovered your Health Gateway account. You may continue to use the service as you did before.
                                </p>
                                <p>Thanks,</p>
                                <p>Health Gateway Team</p>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </body>
                </html>", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HTML", "HG_Donotreply@gov.bc.ca", "AccountRecovered", 10, "Health Gateway Account Recovered", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("d9898318-4e53-4074-9979-5d24bd370055"), @"<!doctype html>
                <html lang=""en"">
                <head>
                </head>
                <body style=""margin:0"">
                    <table cellspacing=""0"" align=""left"" width=""100%"" style=""margin:0;color:#707070;font-family:Helvetica;font-size:12px;"">
                        <tr style=""background:#036;"">
                            <th width=""45""></th>
                            <th width=""350"" align=""left"" style=""text-align:left;"">
                                <div role=""img"" aria-label=""Health Gateway Logo"">
                                    <img src=""${host}/Logo.png"" alt=""Health Gateway Logo"" />
                                </div>
                            </th>
                            <th width=""""></th>
                        </tr>
                        <tr>
                            <td colspan=""3"" height=""20""></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <h1 style=""font-size:18px;"">Hi,</h1>
                                <p>
                                    Your Health Gateway account closure has been completed. Your account and personally entered information have been removed from the application. You are welcome to register again for the Health Gateway in the future.
                                </p>
                                <p>Thanks,</p>
                                <p>Health Gateway Team</p>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </body>
                </html>", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HTML", "HG_Donotreply@gov.bc.ca", "AccountRemoved", 10, "Health Gateway Account Closure Complete", "System", new DateTime(2019, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "gateway",
                table: "EmailTemplate",
                keyColumn: "EmailTemplateId",
                keyValue: new Guid("2fe8c825-d4de-4884-be6a-01a97b466425"));

            migrationBuilder.DeleteData(
                schema: "gateway",
                table: "EmailTemplate",
                keyColumn: "EmailTemplateId",
                keyValue: new Guid("79503a38-c14a-4992-b2fe-5586629f552e"));

            migrationBuilder.DeleteData(
                schema: "gateway",
                table: "EmailTemplate",
                keyColumn: "EmailTemplateId",
                keyValue: new Guid("d9898318-4e53-4074-9979-5d24bd370055"));
        }
    }
}