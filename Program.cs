using Microsoft.EntityFrameworkCore;
using Serilog;
using SongLibrary.Interfaces;
using SongLibrary.SongModule.Endpoints;
using SongLibrary.SongModule.Services;
using SongLibrary.SongModule.Data;
using SongLibrary.UserModule;
using SongLibrary;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/song-log-.txt",
        rollingInterval: RollingInterval.Day, // Creates a new file every day
        retainedFileCountLimit: 7)            // Deletes logs older than a week
    .CreateLogger();

builder.Host.UseSerilog();

// here come services
var conString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<SongDb>(opt => opt.UseNpgsql(conString));

builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddSecurityLayer();

builder.Services.AddSwaggerConfig();

var app = builder.Build();

// exception handler
app.UseMyExceptionHandler();

// logging middleware (serilog)
app.UseLogger();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapSongEndpoints();
app.MapAccountEndpoints();

app.Run();
