namespace BlogSphere.Models;

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
