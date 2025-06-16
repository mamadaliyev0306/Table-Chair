using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class adduserproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "Models",
                table: "Product",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_UserId",
                schema: "Models",
                table: "Product",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_User_UserId",
                schema: "Models",
                table: "Product",
                column: "UserId",
                principalSchema: "Models",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_User_UserId",
                schema: "Models",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_UserId",
                schema: "Models",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Models",
                table: "Product");
        }
    }
}
