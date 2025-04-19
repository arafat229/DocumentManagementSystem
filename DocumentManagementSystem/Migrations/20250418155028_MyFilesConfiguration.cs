using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class MyFilesConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_AspNetUsers_userId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "MyFiles");

            migrationBuilder.RenameIndex(
                name: "IX_Files_userId",
                table: "MyFiles",
                newName: "IX_MyFiles_userId");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "MyFiles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "file",
                table: "MyFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyFiles",
                table: "MyFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyFiles_AspNetUsers_userId",
                table: "MyFiles",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyFiles_AspNetUsers_userId",
                table: "MyFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyFiles",
                table: "MyFiles");

            migrationBuilder.DropColumn(
                name: "file",
                table: "MyFiles");

            migrationBuilder.RenameTable(
                name: "MyFiles",
                newName: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_MyFiles_userId",
                table: "Files",
                newName: "IX_Files_userId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Files",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_AspNetUsers_userId",
                table: "Files",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
