using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class AdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BorrowsCount", "DateOfBirth", "DateOfUserCreation", "Email", "FirstName", "LastName", "PasswordHash", "PersonalIdNumber", "PhoneNumber", "RoleId" },
                values: new object[] { new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"), 0, null, new DateOnly(1, 1, 1), "wojciech@gmail.com", "Admin", "Nimda", "AQAAAAEAACcQAAAAEOmrD0A1sC/44LnYA3fWAK6ky0Ama47feu2xmO8PtAcWAkKLYN7Lu8fHS//lmhSRsg==", null, null, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"));
        }
    }
}
