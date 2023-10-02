using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class edited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListItems_ShoppingLists_ShoppingListId",
                table: "ShoppingListItems");

            migrationBuilder.RenameColumn(
                name: "ShoppingListId",
                table: "ShoppingListItems",
                newName: "ShoppinglistId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingListItems_ShoppingListId",
                table: "ShoppingListItems",
                newName: "IX_ShoppingListItems_ShoppinglistId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListItems_ShoppingLists_ShoppinglistId",
                table: "ShoppingListItems",
                column: "ShoppinglistId",
                principalTable: "ShoppingLists",
                principalColumn: "ShoppingListId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListItems_ShoppingLists_ShoppinglistId",
                table: "ShoppingListItems");

            migrationBuilder.RenameColumn(
                name: "ShoppinglistId",
                table: "ShoppingListItems",
                newName: "ShoppingListId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingListItems_ShoppinglistId",
                table: "ShoppingListItems",
                newName: "IX_ShoppingListItems_ShoppingListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListItems_ShoppingLists_ShoppingListId",
                table: "ShoppingListItems",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "ShoppingListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
