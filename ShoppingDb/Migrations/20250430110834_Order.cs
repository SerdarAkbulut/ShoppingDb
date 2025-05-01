using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApi.Migrations
{
    /// <inheritdoc />
    public partial class Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductImages",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Orders",
                newName: "DaireNo");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "OrderItem",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "ApartmanNo",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Cadde",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sehir",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sokak",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmanNo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Cadde",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FullAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Sehir",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Sokak",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "DaireNo",
                table: "Orders",
                newName: "AddressId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderItem",
                newName: "ProductName");

            migrationBuilder.AddColumn<string>(
                name: "ProductImages",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
