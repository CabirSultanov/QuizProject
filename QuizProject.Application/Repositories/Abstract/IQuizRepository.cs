using QuizProject.Application.Models;
using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IQuizRepository
{
    Task<Quiz> CreateQuizAsync(Quiz quiz);
    Task<IEnumerable<Quiz>> GetQuizzesAsync();
    Task<Quiz?> GetQuizByIdAsync(int id);
    Task<Quiz?> UpdateQuizAsync(Quiz quiz);
    Task DeleteQuizAsync(int id);
}