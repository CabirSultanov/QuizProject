using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizProject.Application.Repositories.Abstract;
using QuizProject.Contracts.Requests;
using QuizProject.Application.Models;

namespace QuizProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class QuizController : ControllerBase
{
    private readonly IQuizRepository _quizRepo;
    private readonly IQuestionRepository _questionRepo;
    private readonly IAnswerRepository _answerRepo;
    private readonly IValidator<CreateQuizRequest> _createQuizValidator;
    private readonly IValidator<UpdateQuizRequest> _updateQuizValidator;
    private readonly IValidator<CreateAnswerRequest> _createAnswerValidator;
    
    public QuizController(
        IQuizRepository quizRepo,
        IQuestionRepository questionRepo,
        IAnswerRepository answerRepo,
        IValidator<CreateQuizRequest> createQuizValidator,
        IValidator<UpdateQuizRequest> updateQuizValidator,
        IValidator<CreateQuestionRequest> createQuestionValidator,
        IValidator<CreateAnswerRequest> createAnswerValidator)
    {
        _quizRepo = quizRepo;
        _questionRepo = questionRepo;
        _answerRepo = answerRepo;
        _createQuizValidator = createQuizValidator;
        _updateQuizValidator = updateQuizValidator;
        _createAnswerValidator = createAnswerValidator;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuizzes()
    {
        var quizzes = await _quizRepo.GetQuizzesAsync();
        return quizzes.Any() ? Ok(quizzes) : NoContent();
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
    {
        await _createQuizValidator.ValidateAndThrowAsync(request);
        
        var quiz = new Quiz
        {
            Title = request.Title,
            Description = request.Description,
            DifficultyLevel = request.DifficultyLevel
        };
        
        try
        {
            var createdQuiz = await _quizRepo.CreateQuizAsync(quiz, request.Image);
            
            foreach (var questionRequest in request.Questions)
            {
                var question = new Question
                {
                    Text = questionRequest.Text,
                    QuizId = createdQuiz.Id
                };
                var createdQuestion = await _questionRepo.CreateQuestionAsync(question);
                
                foreach (var answerRequest in questionRequest.Answers)
                {
                    await _createAnswerValidator.ValidateAndThrowAsync(answerRequest);
                    
                    var answer = new Answer
                    {
                        Text = answerRequest.Text,
                        IsCorrect = answerRequest.IsCorrect,
                        QuestionId = createdQuestion.Id
                    };
                    await _answerRepo.CreateAnswerAsync(answer);
                }
            }
            
            return CreatedAtAction(nameof(GetQuiz), new { id = createdQuiz.Id }, createdQuiz);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateQuiz(int id, [FromForm] UpdateQuizRequest request)
    {
        try
        {
            await _updateQuizValidator.ValidateAndThrowAsync(request);
            
            var quiz = await _quizRepo.GetQuizByIdAsync(id);
            
            quiz.Title = request.Title;
            quiz.Description = request.Description;
            quiz.DifficultyLevel = request.DifficultyLevel;
            await _quizRepo.UpdateQuizAsync(quiz, request.Image);
            
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuestion([FromForm] CreateQuestionRequest request)
    {
        var question = new Question()
        {
            Text = request.Text,
            QuizId = request.QuizId
        };
        
        try
        {
            var createdQuestion = await _questionRepo.CreateQuestionAsync(question);
            
            foreach (var answerRequest in request.Answers)
            {
                var answer = new Answer()
                {
                    Text = answerRequest.Text,
                    IsCorrect = answerRequest.IsCorrect,
                    QuestionId = createdQuestion.Id
                };
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