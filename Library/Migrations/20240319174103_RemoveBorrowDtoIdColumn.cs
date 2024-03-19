using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBorrowDtoIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowDtoId",
                table: "Resources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAECqb53oROIGPldGuDy1slaTMsCkNyv4rS8vQmIeIBCfkizQ1YJ/G+MzIMEjU5OywCQ==");
        }
    }
}
