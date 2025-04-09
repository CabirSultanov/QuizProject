using QuizProject.Application.Models;
using QuizProject.Contracts.Requests;

namespace QuizProject.API.Mapping;

public static class ContractMapping
{
    public static User MapToUser(this RegisterRequest request)
    {
        return new User
        {
            Username = request.Username,
            Password = request.Password,
            Salt = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = "+994" + request.PhoneNumber,
            RoleId = request.RoleId,
        };
    }
    
    public static Quiz MapToQuiz(this CreateQuizRequest request)
    {
        return new Quiz
        {
            Title = request.Title,
            Description = request.Description
        };
    }
    
    public static Question MapToQuestion(this CreateQuestionRequest request)
    {
        return new Question
        {
            Text = request.Text,
            QuizId = request.QuizId
        };
    }

    public static Answer MapToAnswer(this CreateAnswerRequest request, int questionId)
    {
        return new Answer
        {
            Text = request.Text,
            IsCorrect = request.IsCorrect,
            QuestionId = questionId
        };
    }
}