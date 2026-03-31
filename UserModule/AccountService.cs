using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SongLibrary.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SongLibrary.UserModule;

class AccountService : IAccountService
{
    private readonly UserManager<AppUser> userManager;

    public AccountService(UserManager<AppUser> manager)
    {
        userManager = manager;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
        };
        var result = await userManager.CreateAsync(user, registerDto.Password);

        return result;
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            return null;

        var authClaims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TseMiySuperSecretniyKlyuchDlyaTokeniv12345!"));

        var token = new JwtSecurityToken(
                issuer: "SongLibraryApp",  // Хто видав
                audience: "SongLibraryClient", // Для кого
                expires: DateTime.Now.AddHours(3), // Час життя (3 години)
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}