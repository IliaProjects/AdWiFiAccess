using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class SharingAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdIsWatched",
                table: "SessionsArchieved");

            migrationBuilder.DropColumn(
                name: "AdIsWatched",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TrialLink",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "OrderShareId",
                table: "SessionsArchieved",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderShareId",
                table: "Sessions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OrdersShare",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Amount = table.Column<string>(nullable: false),
                    Counter = table.Column<int>(nullable: false),
                    Picture = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    PlacedTime = table.Column<DateTime>(nullable: false),
                    IsExecuted = table.Column<bool>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    ExecutedTime = table.Column<DateTime>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersShare", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionsHover",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Mac = table.Column<string>(nullable: false),
                    Ip = table.Column<string>(nullable: false),
                    TrialLink = table.Column<string>(nullable: false),
                    ConnectedTime = table.Column<DateTime>(nullable: false),
                    QuestIsDone = table.Column<bool>(nullable: false),
                    OrderId = table.Column<string>(nullable: false),
                    OrderShareId = table.Column<string>(nullable: false),
                    RouterId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionsHover", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionsHover_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsHover_OrdersShare_OrderShareId",
                        column: x => x.OrderShareId,
                        principalTable: "OrdersShare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionsHover_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionsArchieved_OrderShareId",
                table: "SessionsArchieved",
                column: "OrderShareId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_OrderShareId",
                table: "Sessions",
                column: "OrderShareId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHover_OrderId",
                table: "SessionsHover",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHover_OrderShareId",
                table: "SessionsHover",
                column: "OrderShareId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionsHover_RouterId",
                table: "SessionsHover",
                column: "RouterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_OrdersShare_OrderShareId",
                table: "Sessions",
                column: "OrderShareId",
                principalTable: "OrdersShare",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionsArchieved_OrdersShare_OrderShareId",
                table: "SessionsArchieved",
                column: "OrderShareId",
                principalTable: "OrdersShare",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_OrdersShare_OrderShareId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionsArchieved_OrdersShare_OrderShareId",
                table: "SessionsArchieved");

            migrationBuilder.DropTable(
                name: "SessionsHover");

            migrationBuilder.DropTable(
                name: "OrdersShare");

            migrationBuilder.DropIndex(
                name: "IX_SessionsArchieved_OrderShareId",
                table: "SessionsArchieved");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_OrderShareId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "OrderShareId",
                table: "SessionsArchieved");

            migrationBuilder.DropColumn(
                name: "OrderShareId",
                table: "Sessions");

            migrationBuilder.AddColumn<bool>(
                name: "AdIsWatched",
                table: "SessionsArchieved",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AdIsWatched",
                table: "Sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TrialLink",
                table: "Sessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
