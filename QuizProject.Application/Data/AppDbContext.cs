using Microsoft.EntityFrameworkCore;
using QuizProject.Application.Models;

namespace QuizProject.Application.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public AppDbContext() { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserQuizAttempt>()
            .HasMany(u => u.UserAnswers)
            .WithOne(ua => ua.UserQuizAttempt)
            .HasForeignKey(ua => ua.UserQuizAttemptId);

        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.Question)
            .WithMany()
            .HasForeignKey(ua => ua.QuestionId);

        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.SelectedAnswer)
            .WithMany()
            .HasForeignKey(ua => ua.SelectedAnswerId);
    }
}