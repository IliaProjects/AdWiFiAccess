using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class NrIsUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Routers_Nr",
                table: "Routers",
                column: "Nr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buses_Nr",
                table: "Buses",
                column: "Nr",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routers_Nr",
                table: "Routers");

            migrationBuilder.DropIndex(
                name: "IX_Buses_Nr",
                table: "Buses");
        }
    }
}