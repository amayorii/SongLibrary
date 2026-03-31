using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
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

    static async Task<IResult> CreateSong(ISongService songService, CreateSongDto songDto, ClaimsPrincipal user)
    {
        if (!MiniValidator.TryValidate(songDto, out var errors)) // attribute validation 
            return TypedResults.ValidationProblem(errors);

        // get user id from claims
        var authorId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var authorFullName = user.FindFirstValue(ClaimTypes.GivenName);

        if (string.IsNullOrEmpty(authorId) || string.IsNullOrEmpty(authorFullName))
        {
            return TypedResults.Unauthorized();
        }

        var song = await songService.CreateAsync(songDto, authorId, authorFullName);

        return TypedResults.Created($"/songs/{song.Id}", song);
    }

    static async Task<IResult> UpdateSong(ISongService songService, int id, UpdateSongDto songDto, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var song = await songService.GetOneAsync(id);

        if (song is null)
            return TypedResults.NotFound();

        if (song?.AuthorId != userId)
            return TypedResults.Unauthorized();

        await songService.UpdateAsync(id, songDto);

        return TypedResults.NoContent();
    }

    static async Task<IResult> DeleteSong(ISongService songService, int id, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        var song = await songService.GetOneAsync(id);

        if (song is null)
            return TypedResults.NotFound();

        if (userId != song.AuthorId)
            return TypedResults.Unauthorized();

        await songService.DeleteAsync(id);

        return TypedResults.NoContent();
    }
}