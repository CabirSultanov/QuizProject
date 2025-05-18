using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IUserQuizAttemptRepository
{
    Task<UserQuizAttempt> CreateUserQuizAttemptAsync(UserQuizAttempt attempt);
    Task<IEnumerable<UserQuizAttempt>> GetUserQuizAttemptsAsync(int userId);
}