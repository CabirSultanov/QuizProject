namespace QuizProject.Application.Models;

public class UserAnswer
{
    public int Id { get; set; }
    public bool IsCorrect { get; set; }
    public int UserQuizAttemptId { get; set; }
    public UserQuizAttempt UserQuizAttempt { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    public int SelectedAnswerId { get; set; }
    public Answer SelectedAnswer { get; set; }
}