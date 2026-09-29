using BlogSphere.Models;

namespace BlogSphere.DAL.Interfaces;

public interface IBlogRepository
{
    Task<IEnumerable<Blog>> GetAllAsync();
    Task<Blog?> GetByIdAsync(int id);
    Task<Blog> AddAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(Blog blog);
}
