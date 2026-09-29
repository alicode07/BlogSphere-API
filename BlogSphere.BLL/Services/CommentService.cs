using BlogSphere.BLL.Interfaces;
using BlogSphere.DAL.Interfaces;
using BlogSphere.Models;

namespace BlogSphere.BLL.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBlogRepository _blogRepository;

    public CommentService(ICommentRepository commentRepository, IBlogRepository blogRepository)
    {
        _commentRepository = commentRepository;
        _blogRepository = blogRepository;
    }

    public Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId) =>
        _commentRepository.GetByBlogIdAsync(blogId);

    public async Task<Comment> AddAsync(int blogId, int userId, string content)
    {
        if (await _blogRepository.GetByIdAsync(blogId) == null)
            throw new KeyNotFoundException("Blog not found.");

        var comment = new Comment { BlogId = blogId, UserId = userId, Content = content };
        await _commentRepository.AddAsync(comment);
        return (await _commentRepository.GetByIdAsync(comment.Id))!; // reload with User
    }

    public async Task DeleteAsync(int commentId, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId)
                      ?? throw new KeyNotFoundException("Comment not found.");
        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own comments.");

        await _commentRepository.DeleteAsync(comment);
    }
}
