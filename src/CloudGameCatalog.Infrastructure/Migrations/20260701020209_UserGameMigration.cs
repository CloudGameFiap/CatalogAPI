using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudGameCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserGameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserGames_IdUser_IdGame",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "IdGame",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "UserGames");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserGames",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserGames",
                type: "DATETIME2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "UserGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "UserGames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "UserGames",
                type: "DECIMAL(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserGames",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
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
                name: "IX_UserGames_GameId",
                table: "UserGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_GameId1",
                table: "UserGames",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames",
                columns: new[] { "UserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId1",
                table: "UserGames",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Games_GameId",
                table: "UserGames",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Games_GameId1",
                table: "UserGames",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Users_UserId",
                table: "UserGames",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Users_UserId1",
                table: "UserGames",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Games_GameId",
                table: "UserGames");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Games_GameId1",
                table: "UserGames");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Users_UserId",
                table: "UserGames");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Users_UserId1",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_GameId",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_GameId1",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_UserId1",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserGames");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UserGames",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<Guid>(
                name: "IdGame",
                table: "UserGames",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdUser",
                table: "UserGames",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_IdUser_IdGame",
                table: "UserGames",
                columns: new[] { "IdUser", "IdGame" },
                unique: true);
        }
    }
}
