using BlogSphere.Models;

namespace BlogSphere.BLL.Interfaces;

public interface IBlogService
{
    Task<IEnumerable<Blog>> GetAllAsync();
    Task<Blog?> GetByIdAsync(int id);
    Task<Blog> CreateAsync(int authorId, string title, string content);

    /// <exception cref="KeyNotFoundException"/>
    /// <exception cref="UnauthorizedAccessException">User is not the author.</exception>
    Task<Blog> UpdateAsync(int id, int userId, string title, string content);

    /// <exception cref="KeyNotFoundException"/>
    /// <exception cref="UnauthorizedAccessException">User is not the author.</exception>
    Task DeleteAsync(int id, int userId);
}
