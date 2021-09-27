using Microsoft.EntityFrameworkCore.Migrations;

namespace app.Migrations
{
    public partial class author : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorHash",
                table: "Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorHash",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Posts");
        }
    }
}
