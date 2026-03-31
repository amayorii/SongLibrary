using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SongLibrary.SongModule.Models;
using SongLibrary.UserModule;

namespace SongLibrary.SongModule.Data;

class SongDb : IdentityDbContext<AppUser>
{
    public SongDb(DbContextOptions<SongDb> options) : base(options) { }
    public DbSet<Song> Songs => Set<Song>();
}