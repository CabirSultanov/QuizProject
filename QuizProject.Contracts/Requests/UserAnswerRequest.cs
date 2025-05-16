namespace QuizProject.Contracts.Requests;

public class UserAnswerRequest
{
    public int QuestionId { get; set; }
    public int SelectedAnswerId { get; set; }
}