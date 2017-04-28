using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FeintSite.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionKey",
                columns: table => new
                {
                    SessionKeyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionKey", x => x.SessionKeyId);
                });

            migrationBuilder.CreateTable(
                name: "ExampleModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExamplePropertyInteger = table.Column<int>(nullable: false),
                    ExamplePropertyString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExampleModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionProperty",
                columns: table => new
                {
                    SessionPropertyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    OwnerSessionKeyId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionProperty", x => x.SessionPropertyId);
                    table.ForeignKey(
                        name: "FK_SessionProperty_SessionKey_OwnerSessionKeyId",
                        column: x => x.OwnerSessionKeyId,
                        principalTable: "SessionKey",
                        principalColumn: "SessionKeyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionProperty_OwnerSessionKeyId",
                table: "SessionProperty",
                column: "OwnerSessionKeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionProperty");

            migrationBuilder.DropTable(
                name: "ExampleModel");

            migrationBuilder.DropTable(
                name: "SessionKey");
        }
    }
}
