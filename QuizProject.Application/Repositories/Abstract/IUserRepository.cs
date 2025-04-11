using Microsoft.AspNetCore.Http;
using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IUserRepository
{
    Task<User> RegisterUserAsync(User user, IFormFile? image);
    Task<User> LoginUserAsync(string username, string password);
    Task<User> UpdateUserAsync(User user);
    Task CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    Task RemoveRefreshTokenAsync(string token);
    Task RemoveAllRefreshTokensAsync(int userId);
}