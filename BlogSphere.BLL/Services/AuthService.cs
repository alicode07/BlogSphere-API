using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogSphere.BLL.Interfaces;
using BlogSphere.DAL.Interfaces;
using BlogSphere.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogSphere.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<User> RegisterAsync(string username, string email, string password)
    {
        if (await _userRepository.GetByEmailAsync(email) != null)
            throw new InvalidOperationException("Email is already registered.");
        if (await _userRepository.GetByUsernameAsync(username) != null)
            throw new InvalidOperationException("Username is already taken.");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        return await _userRepository.AddAsync(user);
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var expiry = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 60;

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
