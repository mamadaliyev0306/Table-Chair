using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class addproductyangi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                schema: "Models",
                table: "Product",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                schema: "Models",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalOrders",
                schema: "Models",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                schema: "ordering",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasedPrice",
                schema: "ordering",
                table: "OrderItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                schema: "Models",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                schema: "Models",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "TotalOrders",
                schema: "Models",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                schema: "ordering",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PurchasedPrice",
                schema: "ordering",
                table: "OrderItems");
        }
    }
}
