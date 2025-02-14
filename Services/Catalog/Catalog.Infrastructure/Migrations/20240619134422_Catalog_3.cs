using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Catalog_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "ProductTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "ProductBrands",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "ImageFileDirectories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_Id",
                table: "ProductTypes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id",
                table: "Products",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBrands_Id",
                table: "ProductBrands",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageFileDirectories_Id",
                table: "ImageFileDirectories",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductTypes_Id",
                table: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_Id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductBrands_Id",
                table: "ProductBrands");

            migrationBuilder.DropIndex(
                name: "IX_ImageFileDirectories_Id",
                table: "ImageFileDirectories");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "ProductBrands");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "ImageFileDirectories");
        }
    }
}
