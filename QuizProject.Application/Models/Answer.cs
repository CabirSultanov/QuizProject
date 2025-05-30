using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class Answer
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Text { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
    [Required]
    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}