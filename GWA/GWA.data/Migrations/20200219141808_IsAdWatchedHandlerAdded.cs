using Microsoft.EntityFrameworkCore.Migrations;

namespace GWA.Data.Migrations
{
    public partial class IsAdWatchedHandlerAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Mac",
                table: "Sessions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AdIsWatched",
                table: "Sessions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdIsWatched",
                table: "Sessions");

            migrationBuilder.AlterColumn<string>(
                name: "Mac",
                table: "Sessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
