using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongLibrary.Migrations
{
    /// <inheritdoc />
    public partial class addAlbumProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Album",
                table: "Songs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Album",
                table: "Songs");
        }
    }
}
