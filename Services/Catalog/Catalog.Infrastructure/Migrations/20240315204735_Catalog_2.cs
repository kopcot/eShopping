using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Catalog_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Products_Name",
            //    table: "Products");
            //
            //migrationBuilder.DropIndex(
            //    name: "IX_Products_Summary",
            //    table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ProductTypes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "ProductTypes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ModifiedCount",
                table: "ProductTypes",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Products",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Products",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Products",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ModifiedCount",
                table: "Products",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ProductBrands",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "ProductBrands",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ModifiedCount",
                table: "ProductBrands",
                type: "int unsigned",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ImageFileDirectories",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "ImageFileDirectories",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "ModifiedCount",
                table: "ImageFileDirectories",
                type: "int unsigned",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedCount",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModifiedCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ProductBrands");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ProductBrands");

            migrationBuilder.DropColumn(
                name: "ModifiedCount",
                table: "ProductBrands");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ImageFileDirectories");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ImageFileDirectories");

            migrationBuilder.DropColumn(
                name: "ModifiedCount",
                table: "ImageFileDirectories");

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Products",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Summary",
                table: "Products",
                column: "Summary",
                unique: true);
        }
    }
}
