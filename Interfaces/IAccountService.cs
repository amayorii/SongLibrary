using Microsoft.AspNetCore.Identity;
using SongLibrary.UserModule;

namespace SongLibrary.Interfaces;

interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterDto registerDto);
    Task<string?> LoginAsync(LoginDto loginDto);
}