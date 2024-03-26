using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Basket_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ShoppingCarts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "ShoppingCarts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ModifiedCount",
                table: "ShoppingCarts",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageFile",
                table: "ShoppingCartItems",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ShoppingCartItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "ShoppingCartItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ModifiedCount",
                table: "ShoppingCartItems",
                type: "int unsigned",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "ModifiedCount",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ModifiedCount",
                table: "ShoppingCartItems");

            migrationBuilder.AlterColumn<string>(
                name: "ImageFile",
                table: "ShoppingCartItems",
                type: "longtext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);
        }
    }
}
