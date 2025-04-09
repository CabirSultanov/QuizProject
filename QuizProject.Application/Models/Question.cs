using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class Question
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Text { get; set; }
    [Required]
    public int QuizId { get; set; }
    public Quiz? Quiz { get; set; }
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}