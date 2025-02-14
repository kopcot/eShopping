using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Users_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IpConnection_UserId",
                table: "IpConnection");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "IpConnection",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "IpConnection",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IpConnection_UserId",
                table: "IpConnection",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IpConnection_Users_UserId",
                table: "IpConnection",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IpConnection_Users_UserId",
                table: "IpConnection");

            migrationBuilder.DropIndex(
                name: "IX_IpConnection_UserId",
                table: "IpConnection");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "IpConnection");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "IpConnection",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IpConnection_UserId",
                table: "IpConnection",
                column: "UserId",
                unique: true);
        }
    }
}
