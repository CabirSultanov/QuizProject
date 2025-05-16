using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace QuizProject.Contracts.Requests;

public class CreateQuizRequest
{
    public required string Title { get; init; }
    public string Description { get; init; }
    public IFormFile? Image { get; init; }
    public int DifficultyLevel { get; init; }
    public List<QuizQuestionRequest> Questions { get; init; } = new List<QuizQuestionRequest>();
}