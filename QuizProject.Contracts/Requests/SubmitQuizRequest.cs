namespace QuizProject.Contracts.Requests;

public class SubmitQuizRequest
{
    public List<UserAnswerRequest> Answers { get; set; }
}