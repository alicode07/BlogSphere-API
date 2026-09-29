using BlogSphere.DAL.Context;
using BlogSphere.DAL.Interfaces;
using BlogSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogSphere.DAL.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly BlogSphereDbContext _context;
    public BlogRepository(BlogSphereDbContext context) => _context = context;

    public async Task<IEnumerable<Blog>> GetAllAsync() =>
        await _context.Blogs
            .Include(b => b.Author)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public async Task<Blog?> GetByIdAsync(int id) =>
        await _context.Blogs
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Blog> AddAsync(Blog blog)
    {
        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();
        return blog;
    }

    public async Task UpdateAsync(Blog blog)
    {
        _context.Blogs.Update(blog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Blog blog)
    {
        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
    }
}
