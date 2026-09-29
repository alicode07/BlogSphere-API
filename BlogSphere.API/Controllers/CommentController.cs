using System.Security.Claims;
using BlogSphere.API.DTOs;
using BlogSphere.BLL.Interfaces;
using BlogSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogSphere.API.Controllers;

[ApiController]
[Route("api")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentController(ICommentService commentService) => _commentService = commentService;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static CommentDto ToDto(Comment c) => new()
    {
        Id = c.Id,
        Content = c.Content,
        BlogId = c.BlogId,
        UserId = c.UserId,
        Username = c.User?.Username ?? string.Empty,
        CreatedAt = c.CreatedAt
    };

    [HttpGet("blogs/{blogId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetByBlog(int blogId)
    {
        var comments = await _commentService.GetByBlogIdAsync(blogId);
        return Ok(comments.Select(ToDto));
    }

    [Authorize]
    [HttpPost("blogs/{blogId:int}/comments")]
    public async Task<ActionResult<CommentDto>> Add(int blogId, CommentCreateDto dto)
    {
        try
        {
            var comment = await _commentService.AddAsync(blogId, CurrentUserId, dto.Content);
            return StatusCode(StatusCodes.Status201Created, ToDto(comment));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [Authorize]
    [HttpDelete("comments/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _commentService.DeleteAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
