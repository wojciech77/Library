using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class EditBorrowDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowDtoResources");

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowDay",
                table: "Borrows",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEOF2Kg11WBT/2sK7+35bbz0E8QE22uu2KWRZ8PsZBLij5PXM7biT7akP9+uo7cA3w==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowDay",
                table: "Borrows");

            migrationBuilder.CreateTable(
                name: "BorrowDtoResources",
                columns: table => new
                {
                    BorrowDtoId = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowDtoResources", x => new { x.BorrowDtoId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_BorrowDtoResources_Borrows_BorrowDtoId",
                        column: x => x.BorrowDtoId,
                        principalTable: "Borrows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowDtoResources_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAED6efqGEoT0tZYlyXQ1i5BNA289Q3LyLKibnJaH4E3Sp5LplZfbjnms7cUZhnQUirg==");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowDtoResources_ResourceId",
                table: "BorrowDtoResources",
                column: "ResourceId");
        }
    }
}
