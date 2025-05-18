using QuizProject.Application.Data;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace QuizProject.Application.Repositories.Concrete;

public class UserQuizAttemptRepository : IUserQuizAttemptRepository
{
    private readonly AppDbContext _context;

    public UserQuizAttemptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserQuizAttempt> CreateUserQuizAttemptAsync(UserQuizAttempt attempt)
    {
        _context.UserQuizAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt;
    }

    public async Task<IEnumerable<UserQuizAttempt>> GetUserQuizAttemptsAsync(int userId)
    {
        return await _context.UserQuizAttempts
            .Include(a => a.Quiz)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }
}