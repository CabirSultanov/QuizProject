namespace QuizProject.Contracts.Requests;

public class VerifyEmailRequest
{
    public required int UserId { get; init; }
    public required string Code { get; init; }
} 