namespace SongLibrary.SongModule.Models;

class Song
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? AuthorId { get; set; }
    public string? Album { get; set; }
    public string? Genre { get; set; }
    public bool IsListened { get; set; } = false;
    public int ListenCount { get; set; } = 0;
    public int Rating { get; set; } = 0;
}