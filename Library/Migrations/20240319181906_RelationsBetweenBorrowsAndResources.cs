using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class RelationsBetweenBorrowsAndResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Borrows_BorrowDtoId",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_BorrowDtoId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "BorrowDtoId",
                table: "Resources");

            migrationBuilder.CreateTable(
                name: "BorrowDtoResource",
                columns: table => new
                {
                    BorrowsId = table.Column<int>(type: "int", nullable: false),
                    ResourcesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowDtoResource", x => new { x.BorrowsId, x.ResourcesId });
                    table.ForeignKey(
                        name: "FK_BorrowDtoResource_Borrows_BorrowsId",
                        column: x => x.BorrowsId,
                        principalTable: "Borrows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowDtoResource_Resources_ResourcesId",
                        column: x => x.ResourcesId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                value: "AQAAAAEAACcQAAAAEHoefYXiLubLL86CIhGcgnzvcBP41W1jf/R24Tn4B4dGPgiJwpGwuCaGQbEsKt57SQ==");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowDtoResource_ResourcesId",
                table: "BorrowDtoResource",
                column: "ResourcesId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowDtoResources_ResourceId",
                table: "BorrowDtoResources",
                column: "ResourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowDtoResource");

            migrationBuilder.DropTable(
                name: "BorrowDtoResources");

            migrationBuilder.AddColumn<int>(
                name: "BorrowDtoId",
                table: "Resources",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEBF4I+DutT3c/pyPaZvT+BmQgmWNrY97+RL/vozN4xMwbhPjTc7/n1RepTCwQxcOng==");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_BorrowDtoId",
                table: "Resources",
                column: "BorrowDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_Borrows_BorrowDtoId",
                table: "Resources",
                column: "BorrowDtoId",
                principalTable: "Borrows",
                principalColumn: "Id");
        }
    }
}
