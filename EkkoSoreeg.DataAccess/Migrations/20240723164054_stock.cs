using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkkoSoreeg.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class stock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockType",
                table: "TbProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StockType",
                table: "TbProduct",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
