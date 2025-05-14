using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApi.Migrations
{
    /// <inheritdoc />
    public partial class CartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "size",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "size",
                table: "CartItem");
        }
    }
}
