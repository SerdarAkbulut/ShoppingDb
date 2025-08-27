using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApi.Migrations
{
    /// <inheritdoc />
    public partial class CartCargo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCargoFree",
                table: "CartItem",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCargoFree",
                table: "CartItem");
        }
    }
}
