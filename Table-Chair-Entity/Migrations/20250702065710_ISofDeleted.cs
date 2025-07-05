using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class ISofDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "Product",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "ordering",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "NewsletterSubscription",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Models",
                table: "NewsletterSubscription",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "Faq",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "ContactMessage",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "Category",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "CartItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "Blog",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "ordering",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "NewsletterSubscription");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Models",
                table: "NewsletterSubscription");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "Faq");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "ContactMessage");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "Blog");
        }
    }
}
