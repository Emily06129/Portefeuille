using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portefeuille.Migrations
{
    /// <inheritdoc />
    public partial class AjoutDonneesPredites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonneesPredites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActifId = table.Column<int>(type: "int", nullable: false),
                    Symbole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePrediction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrixPredit = table.Column<float>(type: "real", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonneesPredites", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonneesPredites");
        }
    }
}
