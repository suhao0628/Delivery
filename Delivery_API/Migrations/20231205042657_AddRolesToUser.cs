using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_API.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBaskets_Dishes_DishId",
                table: "OrderBaskets");

            migrationBuilder.DropIndex(
                name: "IX_OrderBaskets_DishId",
                table: "OrderBaskets");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBaskets_DishId",
                table: "OrderBaskets",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderBaskets_Dishes_DishId",
                table: "OrderBaskets",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
