using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class UserQuizAttempt
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int Score { get; set; }
    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }
    [Required]
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}