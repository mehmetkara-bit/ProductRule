using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductRule.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductionCountry = table.Column<string>(type: "TEXT", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "ProductTests",
                columns: table => new
                {
                    ProductTestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTests", x => x.ProductTestId);
                });

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    ProductNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    ShippingCountry = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.ProductNo);
                    table.ForeignKey(
                        name: "FK_ProductDetails_Products_ProductNo",
                        column: x => x.ProductNo,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleDefinitions",
                columns: table => new
                {
                    RuleDefinitionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RuleName = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: true),
                    ShippingCountry = table.Column<string>(type: "TEXT", nullable: true),
                    ProductionCountry = table.Column<string>(type: "TEXT", nullable: true),
                    ProductTestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleDefinitions", x => x.RuleDefinitionId);
                    table.ForeignKey(
                        name: "FK_RuleDefinitions_ProductTests_ProductTestId",
                        column: x => x.ProductTestId,
                        principalTable: "ProductTests",
                        principalColumn: "ProductTestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductRuleMatches",
                columns: table => new
                {
                    ProductRuleMatchId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RuleDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRuleMatches", x => x.ProductRuleMatchId);
                    table.ForeignKey(
                        name: "FK_ProductRuleMatches_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductRuleMatches_RuleDefinitions_RuleDefinitionId",
                        column: x => x.RuleDefinitionId,
                        principalTable: "RuleDefinitions",
                        principalColumn: "RuleDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRuleMatches_ProductId",
                table: "ProductRuleMatches",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRuleMatches_RuleDefinitionId",
                table: "ProductRuleMatches",
                column: "RuleDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleDefinitions_ProductTestId",
                table: "RuleDefinitions",
                column: "ProductTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDetails");

            migrationBuilder.DropTable(
                name: "ProductRuleMatches");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "RuleDefinitions");

            migrationBuilder.DropTable(
                name: "ProductTests");
        }
    }
}
