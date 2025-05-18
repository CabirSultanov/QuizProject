using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> UpdateUserAsync(User user, IFormFile? image);
    Task DeleteUserAsync(int id);
} 