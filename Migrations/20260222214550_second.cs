using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductRule.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Products_ProductNo",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "ProductNo",
                table: "ProductDetails",
                newName: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Products_ProductId",
                table: "ProductDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Products_ProductId",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductDetails",
                newName: "ProductNo");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Products_ProductNo",
                table: "ProductDetails",
                column: "ProductNo",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
