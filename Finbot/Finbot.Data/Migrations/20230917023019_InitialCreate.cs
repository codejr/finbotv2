using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finbot.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    PortfolioId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordUserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CashBalance = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.PortfolioId);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    TradeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PortfolioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Side = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExecutionPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExecutionTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.TradeId);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    LatestPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    PortfolioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_Positions_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PortfolioId",
                table: "Positions",
                column: "PortfolioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Portfolios");
        }
    }
}
