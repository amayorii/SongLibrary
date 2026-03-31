using Microsoft.AspNetCore.Identity;

namespace SongLibrary.UserModule;

class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}