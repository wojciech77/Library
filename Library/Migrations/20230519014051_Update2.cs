using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Borrows",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Borrows",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "returnDay",
                table: "Borrows",
                newName: "ReturnDay");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDay",
                table: "Borrows",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEGD28T8iW3NSFb/mH2FM1bJLe/IhaXYxUZ/dSWWn3T7l5G7BTy4u7Q258CNTtvrKlw==");

            migrationBuilder.CreateIndex(
                name: "IX_Borrows_UserId",
                table: "Borrows",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrows_Users_UserId",
                table: "Borrows",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrows_Users_UserId",
                table: "Borrows");

            migrationBuilder.DropIndex(
                name: "IX_Borrows_UserId",
                table: "Borrows");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Borrows",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Borrows",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ReturnDay",
                table: "Borrows",
                newName: "returnDay");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "returnDay",
                table: "Borrows",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAECuxf+tEqDqpj3ERmlfnv5cW2Lx4sytvOVV4CHCMslO6N9CVaiZyoxyQk/S9FvzrjQ==");
        }
    }
}
