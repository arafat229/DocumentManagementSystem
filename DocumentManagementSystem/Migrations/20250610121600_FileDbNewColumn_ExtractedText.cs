using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FileDbNewColumn_ExtractedText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extractedtext",
                table: "MyFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extractedtext",
                table: "MyFiles");
        }
    }
}
