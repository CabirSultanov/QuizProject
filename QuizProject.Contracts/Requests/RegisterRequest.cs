using Microsoft.AspNetCore.Http;

namespace QuizProject.Contracts.Requests;

public class RegisterRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required int RoleId { get; init; }
}