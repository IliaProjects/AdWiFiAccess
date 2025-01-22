using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class ConnectedTimeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Sessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConnectedTime",
                table: "Sessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectedTime",
                table: "Sessions");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
