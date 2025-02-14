using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Basket_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "IsHardDeleted",
            //    table: "ShoppingCarts",
            //    type: "tinyint(1)",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsHardDeleted",
            //    table: "ShoppingCartItems",
            //    type: "tinyint(1)",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.CreateIndex(
            //    name: "IX_ShoppingCarts_Id",
            //    table: "ShoppingCarts",
            //    column: "Id",
            //    unique: true);
            //
            //migrationBuilder.CreateIndex(
            //    name: "IX_ShoppingCartItems_Id",
            //    table: "ShoppingCartItems",
            //    column: "Id",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_Id",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_Id",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "ShoppingCartItems");
        }
    }
}
