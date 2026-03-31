using SongLibrary.SongModule.Models;

namespace SongLibrary.SongModule.Extensions;

static class SongSearchingFilters
{
    public static IQueryable<Song> ApplySearchFilter(this IQueryable<Song> songs, string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            // splitting search query into lowercased words ("sabrina", "carpen", "tear")
            var keywords = search.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in keywords)
                songs = songs.Where(s => s.Title!.ToLower().Contains(word) || s.Author!.ToLower().Contains(word));
        }

        return songs;
    }

    public static IQueryable<Song> ApplyAuthorFilter(this IQueryable<Song> songs, string? author)
    {
        if (!string.IsNullOrWhiteSpace(author))
            songs = songs.Where(s => s.Author!.ToLower().Contains(author.ToLower()));

        return songs;
    }

    public static IQueryable<Song> ApplyGenreFilter(this IQueryable<Song> songs, string[]? genres)
    {
        if (genres != null && genres.Length != 0)
            songs = songs.Where(s => genres.Select(x => x.ToLower()).Contains(s.Genre!.ToLower()));

        return songs;
    }

    public static IQueryable<Song> ApplyAlbumFilter(this IQueryable<Song> songs, string? album)
    {
        if (!string.IsNullOrWhiteSpace(album))
            songs = songs.Where(s => s.Album!.ToLower().Contains(album));

        return songs;
    }

    public static IQueryable<Song> ApplyIsListenedFilter(this IQueryable<Song> songs, bool? listened)
    {
        if (listened != null)
            songs = songs.Where(s => s.IsListened == listened);

        return songs;
    }

    public static IQueryable<Song> ApplySortingByProperty(this IQueryable<Song> songs, string? prop)
    {
        if (!string.IsNullOrWhiteSpace(prop))
        {
            // choosing sort by prop name
            songs = prop.ToLower() switch
            {
                "author" => songs.OrderBy(s => s.Author),
                "album" => songs.OrderBy(s => s.Album),
                "genre" => songs.OrderBy(s => s.Genre),
                "listencount" => songs.OrderBy(s => s.ListenCount),
                "rating" => songs.OrderBy(s => s.Rating),
                _ => songs.OrderBy(s => s.Title),
            };
        }

        return songs;
    }

    public static IQueryable<Song> ApplyPagination(this IQueryable<Song> songs, int offset, int limit)
    {
        return songs.Skip(offset).Take(limit);
    }
}