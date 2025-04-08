using System.ComponentModel.DataAnnotations;

namespace QuizProject.Application.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
    public string? Salt { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}