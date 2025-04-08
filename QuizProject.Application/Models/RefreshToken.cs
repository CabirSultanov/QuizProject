using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}