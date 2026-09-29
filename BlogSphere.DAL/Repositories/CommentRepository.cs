using BlogSphere.DAL.Context;
using BlogSphere.DAL.Interfaces;
using BlogSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogSphere.DAL.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly BlogSphereDbContext _context;
    public CommentRepository(BlogSphereDbContext context) => _context = context;

    public async Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId) =>
        await _context.Comments
            .Include(c => c.User)
            .Where(c => c.BlogId == blogId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

    public async Task<Comment?> GetByIdAsync(int id) =>
        await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Comment> AddAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}
