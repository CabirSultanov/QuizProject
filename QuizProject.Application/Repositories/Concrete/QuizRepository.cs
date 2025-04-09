using Microsoft.EntityFrameworkCore;
using QuizProject.Application.Data;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;

namespace QuizProject.Application.Repositories.Concrete;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _context;
    
    public QuizRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        
        return quiz;
    }
    
    public async Task<IEnumerable<Quiz>> GetQuizzesAsync()
    {
        return await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .ToListAsync();
    }
    
    public async Task<Quiz?> GetQuizByIdAsync(int id)
    {
        if (!_context.Quizzes.Any(q => q.Id == id))
        {
            throw new KeyNotFoundException("Invalid Quiz ID.");
        }
        return await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);
    }
    
    public async Task<Quiz?> UpdateQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }
    
    public async Task DeleteQuizAsync(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        
        if (quiz is null)
        {
            throw new KeyNotFoundException("Invalid Quiz ID.");
        }
        
        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
    }
}