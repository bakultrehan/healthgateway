﻿//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
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

    public partial class Comment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                schema: "gateway",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 60, nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(nullable: false),
                    xmin = table.Column<uint>(type: "xid", nullable: false),
                    UserProfileId = table.Column<string>(maxLength: 52, nullable: false),
                    Text = table.Column<string>(maxLength: 1000, nullable: true),
                    EntryTypeCode = table.Column<string>(maxLength: 3, nullable: false),
                    ParentEntryId = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "gateway",
                        principalTable: "UserProfile",
                        principalColumn: "UserProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserProfileId",
                schema: "gateway",
                table: "Comment",
                column: "UserProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment",
                schema: "gateway");
        }
    }
}