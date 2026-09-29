using System.ComponentModel.DataAnnotations;

namespace BlogSphere.API.DTOs;

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
}
