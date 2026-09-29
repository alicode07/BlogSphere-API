namespace BlogSphere.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
