using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery_API.Migrations
{
    /// <inheritdoc />
    public partial class FixDishIdInBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Dishes_DishesId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_DishesId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "DishesId",
                table: "Baskets");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_DishId",
                table: "Baskets",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Dishes_DishId",
                table: "Baskets",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Dishes_DishId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_DishId",
                table: "Baskets");

            migrationBuilder.AddColumn<Guid>(
                name: "DishesId",
                table: "Baskets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_DishesId",
                table: "Baskets",
                column: "DishesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Dishes_DishesId",
                table: "Baskets",
                column: "DishesId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
