using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocumentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class DocMangementSytem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "78416a79-874b-41bc-90f8-3492f18721d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d47bcc64-899b-46e3-94b8-00f211e282e5");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "DocManagementSystems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "80334ef5-95b7-4945-9236-9cfb3f00ff9e", null, "Admin", "ADMIN" },
                    { "fe6a1735-1c12-42e9-af56-8d0e2a4032f4", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80334ef5-95b7-4945-9236-9cfb3f00ff9e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe6a1735-1c12-42e9-af56-8d0e2a4032f4");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "DocManagementSystems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "78416a79-874b-41bc-90f8-3492f18721d4", null, "Admin", "ADMIN" },
                    { "d47bcc64-899b-46e3-94b8-00f211e282e5", null, "User", "USER" }
                });
        }
    }
}
