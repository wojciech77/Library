using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceUser");

            migrationBuilder.AddColumn<int>(
                name: "ResourceId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                columns: new[] { "PasswordHash", "ResourceId" },
                values: new object[] { "AQAAAAEAACcQAAAAECuxf+tEqDqpj3ERmlfnv5cW2Lx4sytvOVV4CHCMslO6N9CVaiZyoxyQk/S9FvzrjQ==", null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResourceId",
                table: "Users",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Resources_ResourceId",
                table: "Users",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Resources_ResourceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ResourceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ResourceUser",
                columns: table => new
                {
                    ResourcesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceUser", x => new { x.ResourcesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ResourceUser_Resources_ResourcesId",
                        column: x => x.ResourcesId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b21ea37d-66d2-4c2c-d01c-08db4a9ac978"),
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEEamYkQQMSOX92fRptPl3eFkYq8NYl+8B/xG08uRjfu3mHG3FTTWrOzXZWtwgkRBMQ==");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceUser_UsersId",
                table: "ResourceUser",
                column: "UsersId");
        }
    }
}
