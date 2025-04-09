namespace QuizProject.Contracts.Requests;

public class CreateAnswerRequest
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}