using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ketabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonForRejection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReasonForRejection",
                table: "BookListing",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonForRejection",
                table: "BookListing");
        }
    }
}
