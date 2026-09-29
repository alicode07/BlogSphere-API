using BlogSphere.Models;

namespace BlogSphere.BLL.Interfaces;

public interface IAuthService
{
    /// <exception cref="InvalidOperationException">Email or username already taken.</exception>
    Task<User> RegisterAsync(string username, string email, string password);

    /// <returns>JWT token, or null if credentials are invalid.</returns>
    Task<string?> LoginAsync(string email, string password);
}
