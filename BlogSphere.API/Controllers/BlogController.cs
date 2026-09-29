using System.Security.Claims;
using BlogSphere.API.DTOs;
using BlogSphere.BLL.Interfaces;
using BlogSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogSphere.API.Controllers;

[ApiController]
[Route("api/blogs")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;
    public BlogController(IBlogService blogService) => _blogService = blogService;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static BlogDto ToDto(Blog b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Content = b.Content,
        AuthorId = b.AuthorId,
        AuthorName = b.Author?.Username ?? string.Empty,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogDto>>> GetAll()
    {
        var blogs = await _blogService.GetAllAsync();
        return Ok(blogs.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogDto>> GetById(int id)
    {
        var blog = await _blogService.GetByIdAsync(id);
        return blog == null ? NotFound() : Ok(ToDto(blog));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BlogDto>> Create(BlogCreateDto dto)
    {
        var blog = await _blogService.CreateAsync(CurrentUserId, dto.Title, dto.Content);
        return CreatedAtAction(nameof(GetById), new { id = blog.Id }, ToDto(blog));
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BlogDto>> Update(int id, BlogCreateDto dto)
    {
        try
        {
            var blog = await _blogService.UpdateAsync(id, CurrentUserId, dto.Title, dto.Content);
            return Ok(ToDto(blog));
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _blogService.DeleteAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
