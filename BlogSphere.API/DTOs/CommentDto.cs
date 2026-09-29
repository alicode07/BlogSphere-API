using System.ComponentModel.DataAnnotations;

namespace BlogSphere.API.DTOs;

public class CommentCreateDto
{
    [Required, StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int BlogId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
