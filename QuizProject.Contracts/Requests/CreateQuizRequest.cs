using Microsoft.AspNetCore.Http;

namespace QuizProject.Contracts.Requests;

public class CreateQuizRequest
{
    public required string Title { get; init; }
    public string Description { get; init; }
    public IFormFile? Image { get; init; }
}