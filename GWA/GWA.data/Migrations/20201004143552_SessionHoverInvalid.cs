using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class SessionHoverInvalid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrialLink",
                table: "SessionsHover");

            migrationBuilder.AddColumn<string>(
                name: "TrialLinkPartI",
                table: "SessionsHover",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrialLinkPartII",
                table: "SessionsHover",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SessionsHoverInvalid",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Mac = table.Column<string>(nullable: false),
                    Ip = table.Column<string>(nullable: false),
                    TrialLinkPartI = table.Column<string>(nullable: false),
                    TrialLinkPartII = table.Column<string>(nullable: false),
                    ConnectedTime = table.Column<DateTime>(nullable: false),
                    OrderId = table.Column<string>(nullable: false),
                    OrderShareId = table.Column<string>(nullable: false),
                    RouterId = table.Column<string>(nullable: false),
                    MadeAction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionsHoverInvalid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionsHoverInvalid_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsHoverInvalid_OrdersShare_OrderShareId",
                        column: x => x.OrderShareId,
                        principalTable: "OrdersShare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsHoverInvalid_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHoverInvalid_OrderId",
                table: "SessionsHoverInvalid",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHoverInvalid_OrderShareId",
                table: "SessionsHoverInvalid",
                column: "OrderShareId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHoverInvalid_RouterId",
                table: "SessionsHoverInvalid",
                column: "RouterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionsHoverInvalid");

            migrationBuilder.DropColumn(
                name: "TrialLinkPartI",
                table: "SessionsHover");

            migrationBuilder.DropColumn(
                name: "TrialLinkPartII",
                table: "SessionsHover");

            migrationBuilder.AddColumn<string>(
                name: "TrialLink",
                table: "SessionsHover",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
