using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudGameCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(120)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(120)", nullable: false),
                    ImageUrl = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Genre = table.Column<string>(type: "VARCHAR(60)", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Active = table.Column<bool>(type: "BIT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
