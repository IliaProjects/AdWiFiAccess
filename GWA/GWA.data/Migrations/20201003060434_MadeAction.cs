using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class MadeAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestIsDone",
                table: "SessionsHover");

            migrationBuilder.AddColumn<int>(
                name: "MadeAction",
                table: "SessionsHover",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MadeAction",
                table: "SessionsArchieved",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MadeAction",
                table: "Sessions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MadeAction",
                table: "SessionsHover");

            migrationBuilder.DropColumn(
                name: "MadeAction",
                table: "SessionsArchieved");

            migrationBuilder.DropColumn(
                name: "MadeAction",
                table: "Sessions");

            migrationBuilder.AddColumn<bool>(
                name: "QuestIsDone",
                table: "SessionsHover",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
