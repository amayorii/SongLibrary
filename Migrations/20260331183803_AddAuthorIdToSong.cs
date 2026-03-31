using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorIdToSong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Songs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Songs");
        }
    }
}
