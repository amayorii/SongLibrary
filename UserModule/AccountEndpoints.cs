using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;
using SongLibrary.Interfaces;

namespace SongLibrary.UserModule;

static class AccountEndpoints
{
    public static IEndpointConventionBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var accGroup = app.MapGroup("account");
        accGroup.MapPost("/register", RegisterAsync);
        accGroup.MapPost("/login", LoginAsync);

        return accGroup;
    }

    static async Task<IResult> RegisterAsync(IAccountService accService, RegisterDto registerDto)
    {
        if (!MiniValidator.TryValidate(registerDto, out var errors))
        {
            return TypedResults.BadRequest(errors);
        }

        var result = await accService.RegisterAsync(registerDto);

        if (result.Succeeded)
            return TypedResults.Ok(new { Message = "User registered successfully!" });
        else
            return TypedResults.BadRequest(result.Errors);
    }

    static async Task<IResult> LoginAsync(IAccountService accService, LoginDto loginDto)
    {
        if (!MiniValidator.TryValidate(loginDto, out var errors))
        {
            return TypedResults.BadRequest(errors);
        }

        var token = await accService.LoginAsync(loginDto);

        if (token == null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(new { Token = token });
    }
}