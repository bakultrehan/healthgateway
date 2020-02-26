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

    public partial class UserProfileHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDateTime",
                schema: "gateway",
                table: "UserProfile",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityManagementId",
                schema: "gateway",
                table: "UserProfile",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDateTime",
                schema: "gateway",
                table: "UserProfile",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserProfileHistory",
                schema: "gateway",
                columns: table => new
                {
                    UserProfileHistoryId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: false),
                    xmin = table.Column<uint>(type: "xid", nullable: false),
                    UserProfileId = table.Column<string>(maxLength: 52, nullable: false),
                    AcceptedTermsOfService = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 254, nullable: true),
                    ClosedDateTime = table.Column<DateTime>(nullable: true),
                    IdentityManagementId = table.Column<Guid>(nullable: true),
                    LastLoginDateTime = table.Column<DateTime>(nullable: true),
                    Operation = table.Column<string>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileHistory", x => x.UserProfileHistoryId);
                });

            string schema = "gateway";
            string triggerFunction = @$"
CREATE FUNCTION {schema}.""UserProfileHistoryFunction""()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    NOT LEAKPROOF
AS $BODY$
BEGIN
    IF(TG_OP = 'DELETE') THEN
        INSERT INTO ""UserProfileHistory""(""UserProfileHistoryId"", ""Operation"", ""OperationDateTime"",
                    ""UserProfileId"", ""AcceptedTermsOfService"", ""Email"", ""ClosedDateTime"", ""IdentityManagementId"",						 
				    ""LastLoginDateTime"", ""CreatedBy"", ""CreatedDateTime"", ""UpdatedBy"", ""UpdatedDateTime"") 
		VALUES(uuid_generate_v4(), TG_OP, now(),
               old.""UserProfileId"", old.""AcceptedTermsOfService"", old.""Email"", old.""ClosedDateTime"", old.""IdentityManagementId"",
               old.""LastLoginDateTime"", old.""CreatedBy"", old.""CreatedDateTime"", old.""UpdatedBy"", old.""UpdatedDateTime"");
        RETURN old;
    END IF;
END;$BODY$;
ALTER FUNCTION {schema}.""UserProfileHistoryFunction""()
    OWNER TO gateway;";

            string trigger = @$"
CREATE TRIGGER ""UserProfileHistoryTrigger""
    AFTER DELETE
    ON {schema}.""UserProfile""
    FOR EACH ROW
    EXECUTE PROCEDURE {schema}.""UserProfileHistoryFunction""();";

            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";");
            migrationBuilder.Sql(triggerFunction);
            migrationBuilder.Sql(trigger);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfileHistory",
                schema: "gateway");

            migrationBuilder.DropColumn(
                name: "ClosedDateTime",
                schema: "gateway",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "IdentityManagementId",
                schema: "gateway",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "LastLoginDateTime",
                schema: "gateway",
                table: "UserProfile");

            string schema = "gateway";
            migrationBuilder.Sql(@$"DROP TRIGGER IF EXISTS ""UserProfileHistoryTrigger"" ON {schema}.""UserProfile""");
            migrationBuilder.Sql(@$"DROP FUNCTION IF EXISTS {schema}.""UserProfileHistoryFunction""();");
            migrationBuilder.Sql(@"DROP EXTENSION IF EXISTS ""uuid-ossp"";");
        }
    }
}