using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrows_Users_UserId",
                table: "Borrows");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Borrows_BorrowDtoId",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Resources_ResourceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ResourceId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Borrows",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Borrows",
                newName: "BorrowDto");

            migrationBuilder.RenameIndex(
                name: "IX_Borrows_UserId",
                table: "BorrowDto",
                newName: "IX_BorrowDto_UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "BorrowDto",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowDto",
                table: "BorrowDto",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEN2loDvJphiILDJe4bQ2/DsKiZUtVF89G6uaL1BQG2WtxCNKsRkHPW4CzcqrRAirFA==");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowDto_Users_UserId",
                table: "BorrowDto",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_BorrowDto_BorrowDtoId",
                table: "Resources",
                column: "BorrowDtoId",
                principalTable: "BorrowDto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowDto_Users_UserId",
                table: "BorrowDto");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_BorrowDto_BorrowDtoId",
                table: "Resources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowDto",
                table: "BorrowDto");

            migrationBuilder.RenameTable(
                name: "BorrowDto",
                newName: "Borrows");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowDto_UserId",
                table: "Borrows",
                newName: "IX_Borrows_UserId");

            migrationBuilder.AddColumn<int>(
                name: "ResourceId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Borrows",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Borrows",
                table: "Borrows",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                columns: new[] { "PasswordHash", "ResourceId" },
                values: new object[] { "AQAAAAEAACcQAAAAEFoUQ0mEEBzQ9vEJcXNd04PVQ8nQFA7zw/wCh82I9Q8tod+HkKanboxvvv0PS9GE5g==", null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResourceId",
                table: "Users",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrows_Users_UserId",
                table: "Borrows",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_Borrows_BorrowDtoId",
                table: "Resources",
                column: "BorrowDtoId",
                principalTable: "Borrows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Resources_ResourceId",
                table: "Users",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
