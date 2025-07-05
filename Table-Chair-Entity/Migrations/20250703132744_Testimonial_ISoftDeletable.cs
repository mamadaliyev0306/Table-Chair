using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class Testimonial_ISoftDeletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "Testimonial",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Models",
                table: "Testimonial",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "Testimonial");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Models",
                table: "Testimonial");
        }
    }
}
