using SongLibrary.SongModule.Dtos;
using SongLibrary.SongModule.Models;

namespace SongLibrary.SongModule.Mappers;

static class SongMapper
{
    public static Song ToSong(this CreateSongDto songDto)
    {
        return new()
        {
            Title = songDto.Title,
            Album = songDto.Album,
            Rating = songDto.Rating,
            Genre = songDto.Genre
        };
    }
}