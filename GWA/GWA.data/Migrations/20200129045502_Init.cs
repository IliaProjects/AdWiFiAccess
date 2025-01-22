using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Route = table.Column<string>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Amount = table.Column<string>(nullable: false),
                    Counter = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    PlacedTime = table.Column<DateTime>(nullable: false),
                    ExecutedTime = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Nr = table.Column<int>(nullable: false),
                    Model = table.Column<string>(nullable: false),
                    PlacedTime = table.Column<DateTime>(nullable: false),
                    BusId = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    BusId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routers_Buses_BusId1",
                        column: x => x.BusId1,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OrderId = table.Column<string>(nullable: false),
                    RouterId = table.Column<string>(nullable: false),
                    Mac = table.Column<string>(nullable: true),
                    StartingTime = table.Column<DateTime>(nullable: false),
                    IsOnline = table.Column<bool>(nullable: false),
                    TerminationTime = table.Column<DateTime>(nullable: false),
                    InboundTraffic = table.Column<double>(nullable: false),
                    OutboundTraffic = table.Column<double>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Routers_BusId1",
                table: "Routers",
                column: "BusId1");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_OrderId",
                table: "Sessions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RouterId",
                table: "Sessions",
                column: "RouterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Routers");

            migrationBuilder.DropTable(
                name: "Buses");
        }
    }
}
