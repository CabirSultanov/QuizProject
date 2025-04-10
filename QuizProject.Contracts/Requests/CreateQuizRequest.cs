namespace QuizProject.Contracts.Requests;

public class CreateQuizRequest
{
    public required string Title { get; init; }
    public string Description { get; init; }
}