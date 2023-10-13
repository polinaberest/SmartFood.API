using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFood.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddedFridgeDeinstallationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Filials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FridgeDeinstallationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FulfilledTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FridgeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FridgeDeinstallationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FridgeDeinstallationRequests_Fridges_FridgeId",
                        column: x => x.FridgeId,
                        principalTable: "Fridges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FridgeDeinstallationRequests_FridgeId",
                table: "FridgeDeinstallationRequests",
                column: "FridgeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FridgeDeinstallationRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Filials");
        }
    }
}
