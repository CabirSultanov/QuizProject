namespace QuizProject.Contracts.Requests;

public class CreateQuestionRequest
{
    public string Text { get; set; }
    public int QuizId { get; set; }
    public List<CreateAnswerRequest> Answers { get; set; }
}