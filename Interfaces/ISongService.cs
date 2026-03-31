using SongLibrary.SongModule.Dtos;
using SongLibrary.SongModule.Models;

namespace SongLibrary.Interfaces;

interface ISongService
{
    Task<(IEnumerable<Song> songs, int totalSongs)> GetAllAsync(SongQueryObject query);
    Task<Song?> GetOneAsync(int id);
    Task<Song> CreateAsync(CreateSongDto songDto, string authorId, string authorFullName);
    Task UpdateAsync(Song song, UpdateSongDto updatedSong);
    Task DeleteAsync(Song song);
    Task<Song?> PlaySong(int id);
}