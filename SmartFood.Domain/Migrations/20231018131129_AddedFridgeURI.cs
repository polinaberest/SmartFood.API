using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFood.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedFridgeURI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "URI",
                table: "Fridges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URI",
                table: "Fridges");
        }
    }
}
