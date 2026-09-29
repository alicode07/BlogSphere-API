using BlogSphere.Models;

namespace BlogSphere.BLL.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId);

    /// <exception cref="KeyNotFoundException">Blog not found.</exception>
    Task<Comment> AddAsync(int blogId, int userId, string content);

    /// <exception cref="KeyNotFoundException"/>
    /// <exception cref="UnauthorizedAccessException">User is not the comment owner.</exception>
    Task DeleteAsync(int commentId, int userId);
}
