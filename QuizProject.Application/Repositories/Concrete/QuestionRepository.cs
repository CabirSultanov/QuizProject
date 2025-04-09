using Microsoft.EntityFrameworkCore;
using QuizProject.Application.Data;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;

namespace QuizProject.Application.Repositories.Concrete;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _context;
    
    public QuestionRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Question> CreateQuestionAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        
        return question;
    }
    
    public async Task<Question?> GetQuestionByIdAsync(int id)
    {
        if (!_context.Questions.Any(q => q.Id == id))
        {
            throw new KeyNotFoundException("Invalid Question ID.");
        }
        return await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
}