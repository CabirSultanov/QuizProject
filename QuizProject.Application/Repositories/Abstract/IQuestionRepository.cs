using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IQuestionRepository
{
    Task<Question> CreateQuestionAsync(Question question);
    Task<Question?> GetQuestionByIdAsync(int id);
}