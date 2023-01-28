using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edstem.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class shopping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
