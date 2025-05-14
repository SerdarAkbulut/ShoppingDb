using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApi.Migrations
{
    /// <inheritdoc />
    public partial class ColorAndSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "size",
                table: "CartItem");

            migrationBuilder.AddColumn<int>(
                name: "colorId",
                table: "CartItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sizeId",
                table: "CartItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_colorId",
                table: "CartItem",
                column: "colorId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_sizeId",
                table: "CartItem",
                column: "sizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Colors_colorId",
                table: "CartItem",
                column: "colorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Sizes_sizeId",
                table: "CartItem",
                column: "sizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Colors_colorId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Sizes_sizeId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_colorId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_sizeId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "colorId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "sizeId",
                table: "CartItem");

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
    }
}
