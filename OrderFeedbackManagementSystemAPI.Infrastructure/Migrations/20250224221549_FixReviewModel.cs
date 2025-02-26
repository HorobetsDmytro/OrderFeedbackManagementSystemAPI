using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFeedbackManagementSystemAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixReviewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Products_ProductId",
                table: "Reviews",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Products_ProductId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Reviews");
        }
    }
}
