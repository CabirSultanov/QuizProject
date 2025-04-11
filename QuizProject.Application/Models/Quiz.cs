using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class Quiz
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}