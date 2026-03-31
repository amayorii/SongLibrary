using MiniValidation;
using SongLibrary.Interfaces;
using SongLibrary.SongModule.Dtos;
using SongLibrary.SongModule.Models;

namespace SongLibrary.SongModule.Endpoints;

static class SongEndpoints
{
    public static IEndpointConventionBuilder MapSongEndpoints(this IEndpointRouteBuilder app)
    {
        // endpoint filtering
        var songsGroup = app.MapGroup("/songs").AddEndpointFilter(async (context, next) =>
        {
            var query = context.Arguments.OfType<SongQueryObject>().FirstOrDefault(); // getting query object from context

            if (query != null && query.Limit > 100)
            {
                return TypedResults.BadRequest(new
                {
                    ErrorMessage = "Limit cannot be greater than 100",
                    CurrentLimit = query.Limit
                });
            }

            return await next(context);
        }).RequireAuthorization();

        songsGroup.MapGet("/", GetAllSongs);
        songsGroup.MapGet("/{id}/play", PlaySong);
        songsGroup.MapGet("/{id}", GetOneSong);
        songsGroup.MapPost("/", CreateSong);
        songsGroup.MapPatch("/{id}", UpdateSong);
        songsGroup.MapDelete("/{id}", DeleteSong);

        return songsGroup;
    }

    static async Task<IResult> PlaySong(ISongService songService, int id)
    {
        var song = await songService.PlaySong(id);

        if (song is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(song.ListenCount);

    }

    static async Task<IResult> GetAllSongs(ISongService songService, [AsParameters] SongQueryObject query)
    {
        var (songs, totalSongs) = await songService.GetAllAsync(query);

        return TypedResults.Ok(new
        {
            TotalSongs = totalSongs, // amount of found songs
            query.Offset,
            query.Limit,
            Data = songs // songs
        });
    }

    static async Task<IResult> GetOneSong(ISongService songService, int id)
    {
        var song = await songService.GetOneAsync(id);

        if (song is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(song);
    }

    static async Task<IResult> CreateSong(ISongService songService, CreateSongDto songDto)
    {
        if (!MiniValidator.TryValidate(songDto, out var errors)) // attribute validation 
            return TypedResults.ValidationProblem(errors);

        var song = await songService.CreateAsync(songDto);

        return TypedResults.Created($"/songs/{song.Id}", song);
    }

    static async Task<IResult> UpdateSong(ISongService songService, int id, UpdateSongDto songDto)
    {
        var success = await songService.UpdateAsync(id, songDto);

        if (!success)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }

    static async Task<IResult> DeleteSong(ISongService songService, int id)
    {
        var success = await songService.DeleteAsync(id);

        if (!success)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }
}