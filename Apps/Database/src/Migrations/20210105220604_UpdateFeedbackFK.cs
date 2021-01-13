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
#pragma warning disable CS1591
// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace HealthGateway.Database.Migrations
{
    public partial class UpdateFeedbackFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFeedback_UserProfile_UserProfileId",
                schema: "gateway",
                table: "UserFeedback");

            migrationBuilder.DropIndex(
                name: "IX_UserFeedback_UserProfileId",
                schema: "gateway",
                table: "UserFeedback");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserFeedback_UserProfileId",
                schema: "gateway",
                table: "UserFeedback",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFeedback_UserProfile_UserProfileId",
                schema: "gateway",
                table: "UserFeedback",
                column: "UserProfileId",
                principalSchema: "gateway",
                principalTable: "UserProfile",
                principalColumn: "UserProfileId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}