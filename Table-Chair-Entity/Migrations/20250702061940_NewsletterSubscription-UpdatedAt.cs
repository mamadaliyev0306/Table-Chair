using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class NewsletterSubscriptionUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Models",
                table: "NewsletterSubscription",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Models",
                table: "NewsletterSubscription");
        }
    }
}
