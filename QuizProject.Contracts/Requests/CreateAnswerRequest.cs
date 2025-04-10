namespace QuizProject.Contracts.Requests;

public class CreateAnswerRequest
{
    public required string Text { get; set; }
    public required bool IsCorrect { get; set; }
}