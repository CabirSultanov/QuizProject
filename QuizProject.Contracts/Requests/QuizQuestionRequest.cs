namespace QuizProject.Contracts.Requests;

public class QuizQuestionRequest
{
    public required string Text { get; set; }
    public required List<CreateAnswerRequest> Answers { get; set; }
}