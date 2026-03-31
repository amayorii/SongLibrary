namespace SongLibrary.SongModule.Dtos;

class UpdateSongDto
{
    public string? Title { get; set; }
    public string? Album { get; set; }
    public string? Genre { get; set; }
    public bool? IsListened { get; set; }
    public int? ListenCount { get; set; }
}