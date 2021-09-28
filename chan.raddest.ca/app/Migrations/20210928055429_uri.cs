using Microsoft.EntityFrameworkCore.Migrations;

namespace app.Migrations
{
    public partial class uri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlobId",
                table: "Files",
                newName: "Uri");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "Posts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Files",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_FileId",
                table: "Posts",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Files_FileId",
                table: "Posts",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Files_FileId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_FileId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "Uri",
                table: "Files",
                newName: "BlobId");

            migrationBuilder.AlterColumn<ulong>(
                name: "FileId",
                table: "Posts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
