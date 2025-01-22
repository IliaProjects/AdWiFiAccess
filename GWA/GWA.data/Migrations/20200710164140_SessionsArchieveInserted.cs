using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class SessionsArchieveInserted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InboundTrafficKiB",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "OutboundTrafficKiB",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TerminationTime",
                table: "Sessions");

            migrationBuilder.CreateTable(
                name: "SessionsArchieved",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Mac = table.Column<string>(nullable: false),
                    Ip = table.Column<string>(nullable: false),
                    AdIsWatched = table.Column<bool>(nullable: false),
                    ConnectedTime = table.Column<DateTime>(nullable: false),
                    StartingTime = table.Column<DateTime>(nullable: false),
                    TerminationTime = table.Column<DateTime>(nullable: false),
                    FullSession = table.Column<bool>(nullable: false),
                    InboundTrafficKiB = table.Column<double>(nullable: false),
                    OutboundTrafficKiB = table.Column<double>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    OrderId = table.Column<string>(nullable: false),
                    RouterId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionsArchieved", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionsArchieved_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsArchieved_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionsArchieved_OrderId",
                table: "SessionsArchieved",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsArchieved_RouterId",
                table: "SessionsArchieved",
                column: "RouterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionsArchieved");

            migrationBuilder.AddColumn<double>(
                name: "InboundTrafficKiB",
                table: "Sessions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OutboundTrafficKiB",
                table: "Sessions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TerminationTime",
                table: "Sessions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
