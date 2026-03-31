using System.ComponentModel.DataAnnotations;

namespace SongLibrary.UserModule;

public class RegisterDto
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}

public class LoginDto
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}