using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class EditedShopListt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShoppingListItems");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ShoppingListItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ShoppingListItems");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShoppingListItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
