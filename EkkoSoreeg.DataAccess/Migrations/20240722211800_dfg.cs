using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkkoSoreeg.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dfg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "TbOrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "TbOrderDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "TbOrderDetails");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "TbOrderDetails");
        }
    }
}
