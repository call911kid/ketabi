using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ketabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListingStatus",
                table: "BookListing",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListingStatus",
                table: "BookListing");
        }
    }
}
