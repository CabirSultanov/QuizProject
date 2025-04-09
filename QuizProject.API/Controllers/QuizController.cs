using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizProject.Application.Repositories.Abstract;
using QuizProject.Contracts.Requests;
using QuizProject.API.Mapping;

namespace QuizProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizRepository _quizRepo;
    private readonly IQuestionRepository _questionRepo;
    private readonly IAnswerRepository _answerRepo;
    
    public QuizController(
        IQuizRepository quizRepo,
        IQuestionRepository questionRepo,
        IAnswerRepository answerRepo)
    {
        _quizRepo = quizRepo;
        _questionRepo = questionRepo;
        _answerRepo = answerRepo;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetQuizzes()
    {
        var quizzes = await _quizRepo.GetQuizzesAsync();
        return quizzes.Any() ? Ok(quizzes) : NoContent();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuiz(int id)
    {
        try
        {
            var quiz = await _quizRepo.GetQuizByIdAsync(id);
            return Ok(quiz);
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
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
    {
        var quiz = request.MapToQuiz();
        try
        {
            var createdQuiz = await _quizRepo.CreateQuizAsync(quiz);
            return CreatedAtAction(nameof(GetQuiz), new { id = createdQuiz.Id }, createdQuiz);
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
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizRequest request)
    {
        try
        {
            var quiz = await _quizRepo.GetQuizByIdAsync(id);
            
            quiz!.Title = request.Title;
            await _quizRepo.UpdateQuizAsync(quiz);
            
            return Ok(quiz);
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
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        try
        {
            await _quizRepo.DeleteQuizAsync(id);
            return NoContent();
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
    
    [HttpGet("question/{id}")]
    public async Task<IActionResult> GetQuestion(int id)
    {
        try
        {
            var question = await _questionRepo.GetQuestionByIdAsync(id);
            return Ok(question);
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
    
    [HttpPost("question")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequest request)
    {
        if (!request.Answers.Any(a => a.IsCorrect))
        {
            return BadRequest("A question must have at least one correct answer.");
        }
        
        var question = request.MapToQuestion();
        
        try
        {
            var createdQuestion = await _questionRepo.CreateQuestionAsync(question);
            
            foreach (var answerRequest in request.Answers)
            {
                var answer = answerRequest.MapToAnswer(createdQuestion.Id);
                await _answerRepo.CreateAnswerAsync(answer);
            }
            
            return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
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
}