using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SongLibrary.SongModule.Data;
using SongLibrary.UserModule;

namespace SongLibrary;

static class ProgramExtensions
{
    public static WebApplication UseLogger(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            Stopwatch sw = new();

            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>()!;

            sw.Start();
            await next();
            sw.Stop();

            logger.LogInformation("Processed {Method} {Path} in {Elapsed}ms",
                context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
        });

        return app;
    }

    public static WebApplication UseMyExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(options =>
        {
            options.Run(async context =>
            {
                // get logger & exception from context
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>()!;
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                logger.LogError(exception, "Unhandled error occured");

                // set response to client
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    Error = "Something went wrong on our end",
                    TraceId = context.TraceIdentifier
                });
            });
        });

        return app;
    }

    public static IServiceCollection AddSecurityLayer(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<SongDb>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = "SongLibraryApp",  // Має співпадати з тим, що в LoginAsync
                ValidAudience = "SongLibraryClient", // Має співпадати з тим, що в LoginAsync
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("TseMiySuperSecretniyKlyuchDlyaTokeniv12345!")) // Той самий ключ!
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection builderServices)
    {
        builderServices.AddEndpointsApiExplorer();

        builderServices.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });

        return builderServices;
    }
}