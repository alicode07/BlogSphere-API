using BlogSphere.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogSphere.DAL.Context;

public class BlogSphereDbContext : DbContext
{
    public BlogSphereDbContext(DbContextOptions<BlogSphereDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Blog>(e =>
        {
            e.Property(b => b.Title).HasMaxLength(200).IsRequired();
            e.Property(b => b.Content).IsRequired();
            e.HasOne(b => b.Author)
             .WithMany(u => u.Blogs)
             .HasForeignKey(b => b.AuthorId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.Property(c => c.Content).HasMaxLength(1000).IsRequired();
            e.HasOne(c => c.Blog)
             .WithMany(b => b.Comments)
             .HasForeignKey(c => c.BlogId)
             .OnDelete(DeleteBehavior.Cascade);
            // Restrict avoids SQL Server "multiple cascade paths" error
            e.HasOne(c => c.User)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
