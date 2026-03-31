using Microsoft.EntityFrameworkCore;
using SongLibrary.Interfaces;
using SongLibrary.SongModule.Data;
using SongLibrary.SongModule.Dtos;
using SongLibrary.SongModule.Extensions;
using SongLibrary.SongModule.Mappers;
using SongLibrary.SongModule.Models;

namespace SongLibrary.SongModule.Services;

class SongService : ISongService
{
    readonly SongDb db;

    public SongService(SongDb db)
    {
        this.db = db;
    }

    public async Task<(IEnumerable<Song> songs, int totalSongs)> GetAllAsync(SongQueryObject query)
    {
        var songs = db.Songs.AsQueryable();

        // query filters
        songs = songs.ApplySearchFilter(query.Search)
             .ApplyAuthorFilter(query.Author)
             .ApplyAlbumFilter(query.Album)
             .ApplyGenreFilter(query.Genres)
             .ApplyIsListenedFilter(query.Listened)
             .ApplySortingByProperty(query.Property);

        var total = songs.Count();

        songs = songs.ApplyPagination(query.Offset, query.Limit);

        var results = await songs.ToListAsync();
        return (results, total);
    }

    public async Task<Song?> GetOneAsync(int id)
    {
        return await db.Songs.FindAsync(id);
    }

    public async Task<Song> CreateAsync(CreateSongDto songDto, string authorId)
    {
        var song = songDto.ToSong();
        song.AuthorId = authorId;

        db.Songs.Add(song);
        await db.SaveChangesAsync();

        return song;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var song = await GetOneAsync(id);

        if (song is null) return false;

        db.Songs.Remove(song);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(int id, UpdateSongDto updatedSong)
    {
        var song = await GetOneAsync(id);

        if (song is null) return false;

        song.Title = updatedSong.Title ?? song.Title;
        song.Author = updatedSong.Author ?? song.Author;
        song.Genre = updatedSong.Genre ?? song.Genre;
        song.Album = updatedSong.Album ?? song.Album;
        song.IsListened = updatedSong.IsListened ?? song.IsListened;
        song.ListenCount = updatedSong.ListenCount ?? song.ListenCount;
        song.Rating = updatedSong.Rating ?? song.Rating;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<Song?> PlaySong(int id)
    {
        var song = await GetOneAsync(id);

        if (song is null)
            return null;

        song.ListenCount++;
        song.IsListened = true;
        await db.SaveChangesAsync();

        return song;
    }
}