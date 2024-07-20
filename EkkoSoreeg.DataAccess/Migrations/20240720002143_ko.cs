using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkkoSoreeg.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ko : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbProductColors_TbProduct_ProductId",
                table: "TbProductColors");

            migrationBuilder.DropForeignKey(
                name: "FK_TbProductSizes_TbProduct_ProductId",
                table: "TbProductSizes");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "TbProductSizes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "TbProductColors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TbProductColors_TbProduct_ProductId",
                table: "TbProductColors",
                column: "ProductId",
                principalTable: "TbProduct",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TbProductSizes_TbProduct_ProductId",
                table: "TbProductSizes",
                column: "ProductId",
                principalTable: "TbProduct",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbProductColors_TbProduct_ProductId",
                table: "TbProductColors");

            migrationBuilder.DropForeignKey(
                name: "FK_TbProductSizes_TbProduct_ProductId",
                table: "TbProductSizes");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "TbProductSizes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "TbProductColors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TbProductColors_TbProduct_ProductId",
                table: "TbProductColors",
                column: "ProductId",
                principalTable: "TbProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TbProductSizes_TbProduct_ProductId",
                table: "TbProductSizes",
                column: "ProductId",
                principalTable: "TbProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
