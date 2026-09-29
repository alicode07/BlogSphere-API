using BlogSphere.Models;

namespace BlogSphere.DAL.Interfaces;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId);
    Task<Comment?> GetByIdAsync(int id);
    Task<Comment> AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
}
