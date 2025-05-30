using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;
using QuizProject.Contracts.Requests;

namespace QuizProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserQuizController : ControllerBase
{
    private readonly IQuizRepository _quizRepo;
    private readonly IUserQuizAttemptRepository _userQuizAttemptRepo;
    
    public UserQuizController(IQuizRepository quizRepo, IUserQuizAttemptRepository userQuizAttemptRepo)
    {
        _quizRepo = quizRepo;
        _userQuizAttemptRepo = userQuizAttemptRepo;
    }
    
    /// <summary>
    /// Get one random quiz by difficulty level
    /// </summary>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    [HttpGet("random")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRandomQuiz(int difficulty)
    {
        try
        {
            var quiz = await _quizRepo.GetRandomQuizByDifficultyAsync(difficulty);

            return Ok(quiz);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    /// <summary>
    /// Submit selected answers for a quiz
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{id}/submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitQuiz(int id, [FromBody] SubmitQuizRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var quiz = await _quizRepo.GetQuizByIdAsync(id);
            
            var correctAnswers = quiz.Questions.ToDictionary(q => q.Id, q => q.Answers.First(a => a.IsCorrect).Id);
            var attempt = new UserQuizAttempt
            {
                UserId = userId,
                QuizId = id,
                UserAnswers = new List<UserAnswer>()
            };
            
            int score = 0;
            foreach (var userAnswerRequest in request.Answers)
            {
                bool isCorrect = userAnswerRequest.SelectedAnswerId == correctAnswers[userAnswerRequest.QuestionId];
                if (isCorrect)
                {
                    score++;
                }
                
                attempt.UserAnswers.Add(new UserAnswer
                {
                    QuestionId = userAnswerRequest.QuestionId,
                    SelectedAnswerId = userAnswerRequest.SelectedAnswerId,
                    IsCorrect = isCorrect
                });
            }
            
            attempt.Score = score;
            await _userQuizAttemptRepo.CreateUserQuizAttemptAsync(attempt);
            
            return Ok(new { score, quiz.Questions.Count });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Get all quiz attempts of the current user
    /// </summary>
    /// <returns></returns>
    [HttpGet("attempts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAttempts()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var attempts = await _userQuizAttemptRepo.GetUserQuizAttemptsAsync(userId);
        var result = attempts.Select(a => new
        {
            quizTitle = a.Quiz!.Title,
            difficultyLevel = a.Quiz!.DifficultyLevel,
            score = a.Score,
            date = DateTime.UtcNow.ToString("o")
        });
        return Ok(result);
    }
}