using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portefeuille.Migrations
{
    /// <inheritdoc />
    public partial class MigrationPropre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ScoreSharp",
                table: "Portfolio_client",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "RisqueVolatilite",
                table: "Portfolio_client",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "RendementPrevu",
                table: "Portfolio_client",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<double>(
                name: "Budget",
                table: "Portfolio_client",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CVaR95",
                table: "Portfolio_client",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "NiveauRisque",
                table: "Portfolio_client",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PoidsJson",
                table: "Portfolio_client",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "VaR95",
                table: "Portfolio_client",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

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

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Portfolio_client");

            migrationBuilder.DropColumn(
                name: "CVaR95",
                table: "Portfolio_client");

            migrationBuilder.DropColumn(
                name: "NiveauRisque",
                table: "Portfolio_client");

            migrationBuilder.DropColumn(
                name: "PoidsJson",
                table: "Portfolio_client");

            migrationBuilder.DropColumn(
                name: "VaR95",
                table: "Portfolio_client");

            migrationBuilder.AlterColumn<float>(
                name: "ScoreSharp",
                table: "Portfolio_client",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "RisqueVolatilite",
                table: "Portfolio_client",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "RendementPrevu",
                table: "Portfolio_client",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            
        }
    }
}
