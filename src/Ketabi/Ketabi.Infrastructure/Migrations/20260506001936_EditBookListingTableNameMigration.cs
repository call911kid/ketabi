using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ketabi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditBookListingTableNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookListingTags_UserBooks_BookListingId",
                table: "BookListingTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_UserBooks_ListingId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_UserBooks_OfferedListingId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_Categories_CategoryId",
                table: "UserBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_Users_UserId",
                table: "UserBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBooks",
                table: "UserBooks");

            migrationBuilder.RenameTable(
                name: "UserBooks",
                newName: "BookListing");

            migrationBuilder.RenameIndex(
                name: "IX_UserBooks_UserId",
                table: "BookListing",
                newName: "IX_BookListing_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserBooks_CategoryId",
                table: "BookListing",
                newName: "IX_BookListing_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookListing",
                table: "BookListing",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookListing_Categories_CategoryId",
                table: "BookListing",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookListing_Users_UserId",
                table: "BookListing",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookListingTags_BookListing_BookListingId",
                table: "BookListingTags",
                column: "BookListingId",
                principalTable: "BookListing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_BookListing_ListingId",
                table: "Requests",
                column: "ListingId",
                principalTable: "BookListing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_BookListing_OfferedListingId",
                table: "Requests",
                column: "OfferedListingId",
                principalTable: "BookListing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookListing_Categories_CategoryId",
                table: "BookListing");

            migrationBuilder.DropForeignKey(
                name: "FK_BookListing_Users_UserId",
                table: "BookListing");

            migrationBuilder.DropForeignKey(
                name: "FK_BookListingTags_BookListing_BookListingId",
                table: "BookListingTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_BookListing_ListingId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_BookListing_OfferedListingId",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookListing",
                table: "BookListing");

            migrationBuilder.RenameTable(
                name: "BookListing",
                newName: "UserBooks");

            migrationBuilder.RenameIndex(
                name: "IX_BookListing_UserId",
                table: "UserBooks",
                newName: "IX_UserBooks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookListing_CategoryId",
                table: "UserBooks",
                newName: "IX_UserBooks_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBooks",
                table: "UserBooks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookListingTags_UserBooks_BookListingId",
                table: "BookListingTags",
                column: "BookListingId",
                principalTable: "UserBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_UserBooks_ListingId",
                table: "Requests",
                column: "ListingId",
                principalTable: "UserBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_UserBooks_OfferedListingId",
                table: "Requests",
                column: "OfferedListingId",
                principalTable: "UserBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_Categories_CategoryId",
                table: "UserBooks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_Users_UserId",
                table: "UserBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
