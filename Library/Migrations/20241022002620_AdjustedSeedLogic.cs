using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class AdjustedSeedLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3c1b840c-7bd3-4f05-82a2-ded51c116f57"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BorrowsCount", "DateOfBirth", "DateOfUserCreation", "Email", "FirstName", "LastName", "PasswordHash", "PersonalIdNumber", "PhoneNumber", "RoleId" },
                values: new object[] { new Guid("3c1b840c-7bd3-4f05-82a2-ded51c116f57"), 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "wojciech@gmail.com", "Admin", "Admin", "AQAAAAEAACcQAAAAEOXbPQ9/reBLC+d3j+nJuk0qqtl35uSHnBuAubn4G++GMOSCbDz5UXizOBhF7Gdfug==", null, null, 3 });
        }
    }
}
