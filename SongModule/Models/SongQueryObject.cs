using Microsoft.AspNetCore.Mvc;

namespace SongLibrary.SongModule.Models;

record SongQueryObject([FromQuery(Name = "search")] string? Search,
                    [FromQuery(Name = "genre")] string[]? Genres,
                    [FromQuery(Name = "listened")] bool? Listened,
                    [FromQuery(Name = "author")] string? Author,
                    [FromQuery(Name = "album")] string? Album,
                    [FromQuery(Name = "orderBy")] string? Property,
                    [FromQuery(Name = "offset")] int Offset = 0,
                    [FromQuery(Name = "limit")] int Limit = 20);