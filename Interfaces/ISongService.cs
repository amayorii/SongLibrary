using SongLibrary.SongModule.Dtos;
using SongLibrary.SongModule.Models;

namespace SongLibrary.Interfaces;

interface ISongService
{
    Task<(IEnumerable<Song> songs, int totalSongs)> GetAllAsync(SongQueryObject query);
    Task<Song?> GetOneAsync(int id);
    Task<Song> CreateAsync(CreateSongDto songDto, string authorId);
    Task<bool> UpdateAsync(int id, UpdateSongDto updatedSong);
    Task<bool> DeleteAsync(int id);
    Task<Song?> PlaySong(int id);
}