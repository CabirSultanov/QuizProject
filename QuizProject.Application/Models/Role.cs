using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class Role
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}