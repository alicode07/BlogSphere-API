using BlogSphere.DAL.Context;
using BlogSphere.DAL.Interfaces;
using BlogSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogSphere.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BlogSphereDbContext _context;
    public UserRepository(BlogSphereDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
