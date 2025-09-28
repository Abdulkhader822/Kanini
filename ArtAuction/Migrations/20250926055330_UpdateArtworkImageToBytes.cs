using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAuction.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArtworkImageToBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Artworks");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Artworks",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Artworks",
                keyColumn: "ArtworkId",
                keyValue: 1,
                column: "ImageData",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Artworks");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Artworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Artworks",
                keyColumn: "ArtworkId",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/sunsetimage.jpeg");
        }
    }
}
