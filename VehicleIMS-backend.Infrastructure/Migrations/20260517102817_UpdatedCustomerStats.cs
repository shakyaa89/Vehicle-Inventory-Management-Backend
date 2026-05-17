using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleIMS_backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCustomerStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditBalance",
                table: "CustomerStats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditBalance",
                table: "CustomerStats");
        }
    }
}
