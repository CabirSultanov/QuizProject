using QuizProject.Application.Data;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;

namespace QuizProject.Application.Repositories.Concrete;

public class AnswerRepository : IAnswerRepository
{
    private readonly AppDbContext _context;
    
    public AnswerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Answer> CreateAnswerAsync(Answer answer)
    {
        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();
        
        return answer;
    }
}