using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkkoSoreeg.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dkddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "TbShoppingCarts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "TbShoppingCarts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "TbShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "TbShoppingCarts");
        }
    }
}
