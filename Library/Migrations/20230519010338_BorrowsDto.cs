using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class BorrowsDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BorrowDtoId",
                table: "Resources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Borrows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    returnDay = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrows", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEamYkQQMSOX92fRptPl3eFkYq8NYl+8B/xG08uRjfu3mHG3FTTWrOzXZWtwgkRBMQ==");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Borrows_BorrowDtoId",
                table: "Resources");

            migrationBuilder.DropTable(
                name: "Borrows");

            migrationBuilder.DropIndex(
                name: "IX_Resources_BorrowDtoId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "BorrowDtoId",
                table: "Resources");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEOmrD0A1sC/44LnYA3fWAK6ky0Ama47feu2xmO8PtAcWAkKLYN7Lu8fHS//lmhSRsg==");
        }
    }
}
