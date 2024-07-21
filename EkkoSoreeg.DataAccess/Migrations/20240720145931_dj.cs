using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkkoSoreeg.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeMappings_TbProductSizes_ProductColorId",
                table: "ProductSizeMappings");

            migrationBuilder.RenameColumn(
                name: "ProductColorId",
                table: "ProductSizeMappings",
                newName: "ProductSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeMappings_ProductColorId",
                table: "ProductSizeMappings",
                newName: "IX_ProductSizeMappings_ProductSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeMappings_TbProductSizes_ProductSizeId",
                table: "ProductSizeMappings",
                column: "ProductSizeId",
                principalTable: "TbProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSizeMappings_TbProductSizes_ProductSizeId",
                table: "ProductSizeMappings");

            migrationBuilder.RenameColumn(
                name: "ProductSizeId",
                table: "ProductSizeMappings",
                newName: "ProductColorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSizeMappings_ProductSizeId",
                table: "ProductSizeMappings",
                newName: "IX_ProductSizeMappings_ProductColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSizeMappings_TbProductSizes_ProductColorId",
                table: "ProductSizeMappings",
                column: "ProductColorId",
                principalTable: "TbProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
