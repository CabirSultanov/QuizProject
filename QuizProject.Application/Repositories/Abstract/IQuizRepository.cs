using Microsoft.AspNetCore.Http;
using QuizProject.Application.Models;

namespace QuizProject.Application.Repositories.Abstract;

public interface IQuizRepository
{
    Task<Quiz> CreateQuizAsync(Quiz quiz, IFormFile? image);
    Task<IEnumerable<Quiz>> GetQuizzesAsync();
    Task<Quiz?> GetQuizByIdAsync(int id);
    Task<Quiz?> UpdateQuizAsync(Quiz quiz, IFormFile? image);
    Task DeleteQuizAsync(int id);
    Task<Quiz?> GetRandomQuizByDifficultyAsync(int difficulty);
}