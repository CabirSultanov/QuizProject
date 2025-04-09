namespace QuizProject.Contracts.Requests;

public class CreateQuizRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}