using Microsoft.AspNetCore.Http;

namespace QuizProject.Contracts.Requests;

public class UpdateUserRequest
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public int RoleId { get; init; }
    public string? Password { get; init; }
    public IFormFile? Image { get; init; }
} 