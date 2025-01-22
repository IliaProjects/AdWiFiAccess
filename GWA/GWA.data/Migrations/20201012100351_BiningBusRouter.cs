using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class BiningBusRouter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routers_Buses_BusId",
                table: "Routers");

            migrationBuilder.DropIndex(
                name: "IX_Routers_BusId",
                table: "Routers");

            migrationBuilder.DropColumn(
                name: "BusId",
                table: "Routers");

            migrationBuilder.CreateTable(
                name: "BindingRouterBuses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RouterId = table.Column<string>(nullable: false),
                    BusId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BindingRouterBuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BindingRouterBuses_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BindingRouterBuses_Routers_RouterId",
                        column: x => x.RouterId,
                        principalTable: "Routers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BindingRouterBuses_BusId",
                table: "BindingRouterBuses",
                column: "BusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BindingRouterBuses_RouterId",
                table: "BindingRouterBuses",
                column: "RouterId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BindingRouterBuses");

            migrationBuilder.AddColumn<string>(
                name: "BusId",
                table: "Routers",
                type: "text",
                nullable: false,
                defaultValue: "");

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
    }
}
