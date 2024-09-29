using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullAtReturnDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDay",
                table: "Borrows",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAED6efqGEoT0tZYlyXQ1i5BNA289Q3LyLKibnJaH4E3Sp5LplZfbjnms7cUZhnQUirg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDay",
                table: "Borrows",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEBmu//PjbmgUbEw6gwYyT8oMdhfJFDQqKbq/GukFpa3OxfOKXiORgN4AkFwwRKcTRw==");
        }
    }
}
