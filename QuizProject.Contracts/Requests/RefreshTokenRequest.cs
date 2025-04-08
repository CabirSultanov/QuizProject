namespace QuizProject.Contracts.Requests;

public class RefreshTokenRequest
{
    public required string Token { get; init; }
}