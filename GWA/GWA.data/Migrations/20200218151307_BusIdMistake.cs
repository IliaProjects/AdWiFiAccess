using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class BusIdMistake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routers_Buses_BusId1",
                table: "Routers");

            migrationBuilder.DropIndex(
                name: "IX_Routers_BusId1",
                table: "Routers");

            migrationBuilder.DropColumn(
                name: "BusId1",
                table: "Routers");

            migrationBuilder.AlterColumn<string>(
                name: "BusId",
                table: "Routers",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Routers_BusId",
                table: "Routers",
                column: "BusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routers_Buses_BusId",
                table: "Routers",
                column: "BusId",
                principalTable: "Buses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routers_Buses_BusId",
                table: "Routers");

            migrationBuilder.DropIndex(
                name: "IX_Routers_BusId",
                table: "Routers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BusId",
                table: "Routers",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "BusId1",
                table: "Routers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routers_BusId1",
                table: "Routers",
                column: "BusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Routers_Buses_BusId1",
                table: "Routers",
                column: "BusId1",
                principalTable: "Buses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
