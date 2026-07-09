using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudGameCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserGameRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Games_GameId1",
                table: "UserGames");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Users_UserId1",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_GameId1",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_UserId1",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserGames");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "UserGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_GameId1",
                table: "UserGames",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId1",
                table: "UserGames",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Games_GameId1",
                table: "UserGames",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Users_UserId1",
                table: "UserGames",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
