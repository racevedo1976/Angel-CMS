using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Angelo.Connect.Web.Migrations
{
    public partial class DbLog_Migration20180126 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.CreateTable(
                name: "LogEvent",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    EventCode = table.Column<int>(nullable: false),
                    EventName = table.Column<string>(nullable: true),
                    LogLevel = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ResourceId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentEvent",
                schema: "log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DbLogEventId = table.Column<string>(nullable: true),
                    DocumentId = table.Column<string>(nullable: true),
                    EventId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentEvent_LogEvent_EventId",
                        column: x => x.EventId,
                        principalSchema: "log",
                        principalTable: "LogEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentEvent_EventId",
                schema: "log",
                table: "DocumentEvent",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentEvent",
                schema: "log");

            migrationBuilder.DropTable(
                name: "LogEvent",
                schema: "log");
        }
    }
}
