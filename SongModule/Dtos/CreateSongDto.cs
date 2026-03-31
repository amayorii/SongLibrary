using System.ComponentModel.DataAnnotations;

namespace SongLibrary.SongModule.Dtos;

class CreateSongDto
{
    [StringLength(40), Required]
    public required string Title { get; set; }

    [StringLength(30), Required]
    public required string Author { get; set; }

    [StringLength(30)]
    public string? Album { get; set; } = null;

    [StringLength(20), Required]
    public required string Genre { get; set; }

    [Range(1, 10)]
    public int Rating { get; set; } = 0;
}