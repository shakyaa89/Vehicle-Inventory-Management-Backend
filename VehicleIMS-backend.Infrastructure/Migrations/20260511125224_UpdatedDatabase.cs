using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleIMS_backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "PartRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PartRequests_CustomerId",
                table: "PartRequests",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartRequests_AspNetUsers_CustomerId",
                table: "PartRequests",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartRequests_AspNetUsers_CustomerId",
                table: "PartRequests");

            migrationBuilder.DropIndex(
                name: "IX_PartRequests_CustomerId",
                table: "PartRequests");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PartRequests");
        }
    }
}
