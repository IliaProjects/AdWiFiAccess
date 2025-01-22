using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class trialLinkAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrialLink",
                table: "Sessions",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrialLink",
                table: "Sessions");
        }
    }
}
