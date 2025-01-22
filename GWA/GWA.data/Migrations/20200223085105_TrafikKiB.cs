using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class TrafikKiB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InboundTraffic",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "OutboundTraffic",
                table: "Sessions");

            migrationBuilder.AddColumn<double>(
                name: "InboundTrafficKiB",
                table: "Sessions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OutboundTrafficKiB",
                table: "Sessions",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InboundTrafficKiB",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "OutboundTrafficKiB",
                table: "Sessions");

            migrationBuilder.AddColumn<double>(
                name: "InboundTraffic",
                table: "Sessions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OutboundTraffic",
                table: "Sessions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
