using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenfieldLocalHubWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveOffersToLoyaltyAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveOffers",
                table: "loyaltyAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveOffers",
                table: "loyaltyAccount");
        }
    }
}
