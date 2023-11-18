using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_API.Migrations
{
    /// <inheritdoc />
    public partial class AddDishIdAsFKinOrderBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DishId",
                table: "OrderBaskets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderBaskets_Dishes_DishId",
                table: "OrderBaskets");

            migrationBuilder.DropIndex(
                name: "IX_OrderBaskets_DishId",
                table: "OrderBaskets");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "OrderBaskets");
        }
    }
}
