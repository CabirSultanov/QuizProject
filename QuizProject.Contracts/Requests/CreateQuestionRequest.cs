namespace QuizProject.Contracts.Requests;

public class CreateQuestionRequest
{
    public required string Text { get; set; }
    public required int QuizId { get; set; }
    public required List<CreateAnswerRequest> Answers { get; set; }
}