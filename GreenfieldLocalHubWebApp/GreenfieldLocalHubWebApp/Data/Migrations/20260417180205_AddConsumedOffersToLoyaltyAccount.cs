using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenfieldLocalHubWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumedOffersToLoyaltyAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConsumedOffers",
                table: "loyaltyAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumedOffers",
                table: "loyaltyAccount");
        }
    }
}
