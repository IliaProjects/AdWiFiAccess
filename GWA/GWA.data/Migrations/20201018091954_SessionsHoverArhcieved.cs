using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class SessionsHoverArhcieved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionsHoverArchieved",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Mac = table.Column<string>(nullable: false),
                    Ip = table.Column<string>(nullable: false),
                    ConnectedTime = table.Column<DateTime>(nullable: false),
                    OrderId = table.Column<string>(nullable: false),
                    OrderShareId = table.Column<string>(nullable: false),
                    RouterId = table.Column<string>(nullable: false),
                    MadeAction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionsHoverArchieved", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionsHoverArchieved_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsHoverArchieved_OrdersShare_OrderShareId",
                        column: x => x.OrderShareId,
                        principalTable: "OrdersShare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsHoverArchieved_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHoverArchieved_OrderId",
                table: "SessionsHoverArchieved",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHoverArchieved_OrderShareId",
                table: "SessionsHoverArchieved",
                column: "OrderShareId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHoverArchieved_RouterId",
                table: "SessionsHoverArchieved",
                column: "RouterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionsHoverArchieved");
        }
    }
}
