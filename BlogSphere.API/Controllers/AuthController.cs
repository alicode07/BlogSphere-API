using BlogSphere.API.DTOs;
using BlogSphere.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        try
        {
            var user = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);
            return StatusCode(StatusCodes.Status201Created, new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null) return Unauthorized(new { message = "Invalid email or password." });
        return Ok(new TokenDto { Token = token });
    }
}
