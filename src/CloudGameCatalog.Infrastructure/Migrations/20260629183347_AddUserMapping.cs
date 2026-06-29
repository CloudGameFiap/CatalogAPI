using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudGameCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdGame = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(120)", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(120)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Active = table.Column<bool>(type: "BIT", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    IsAdmin = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_IdUser_IdGame",
                table: "UserGames",
                columns: new[] { "IdUser", "IdGame" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGames");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
