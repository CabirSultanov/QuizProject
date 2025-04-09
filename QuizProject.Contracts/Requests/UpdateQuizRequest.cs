namespace QuizProject.Contracts.Requests;

public class UpdateQuizRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}