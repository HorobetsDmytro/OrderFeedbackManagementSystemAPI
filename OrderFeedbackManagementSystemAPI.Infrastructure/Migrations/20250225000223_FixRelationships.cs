using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFeedbackManagementSystemAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderId",
                table: "Reviews",
                column: "OrderId",
                unique: true);
        }
    }
}
