using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IAnswerRepository
{
    Task<Answer> CreateAnswerAsync(Answer answer);
}