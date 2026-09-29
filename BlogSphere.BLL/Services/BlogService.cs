using BlogSphere.BLL.Interfaces;
using BlogSphere.DAL.Interfaces;
using BlogSphere.Models;

namespace BlogSphere.BLL.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;
    public BlogService(IBlogRepository blogRepository) => _blogRepository = blogRepository;

    public Task<IEnumerable<Blog>> GetAllAsync() => _blogRepository.GetAllAsync();

    public Task<Blog?> GetByIdAsync(int id) => _blogRepository.GetByIdAsync(id);

    public async Task<Blog> CreateAsync(int authorId, string title, string content)
    {
        var blog = new Blog { AuthorId = authorId, Title = title, Content = content };
        await _blogRepository.AddAsync(blog);
        return (await _blogRepository.GetByIdAsync(blog.Id))!; // reload with Author
    }

    public async Task<Blog> UpdateAsync(int id, int userId, string title, string content)
    {
        var blog = await _blogRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Blog not found.");
        if (blog.AuthorId != userId)
            throw new UnauthorizedAccessException("You can only edit your own blogs.");

        blog.Title = title;
        blog.Content = content;
        blog.UpdatedAt = DateTime.UtcNow;
        await _blogRepository.UpdateAsync(blog);
        return blog;
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var blog = await _blogRepository.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException("Blog not found.");
        if (blog.AuthorId != userId)
            throw new UnauthorizedAccessException("You can only delete your own blogs.");

        await _blogRepository.DeleteAsync(blog);
    }
}
