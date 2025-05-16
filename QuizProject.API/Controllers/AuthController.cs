using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;
using QuizProject.Application.Tokens;
using QuizProject.Contracts.Requests;
using QuizProject.Contracts.Responses;
using QuizProject.API.Mapping;

namespace QuizProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _repo;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    
    public AuthController(IAuthRepository repo, ITokenGenerator tokenGenerator, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
    {
        _repo = repo;
        _tokenGenerator = tokenGenerator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }
    
    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);
        
        var user = request.MapToUser();
        try
        {
            var registeredUser = await _repo.RegisterUserAsync(user, request.Image);
            return CreatedAtAction(nameof(Register), new { id = registeredUser.Id },
                $"User with ID {registeredUser.Id} registered successfully.");
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Login user and generate a pair of tokens
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        try
        {
            await _loginValidator.ValidateAndThrowAsync(request);
            
            var user = await _repo.LoginUserAsync(request.Username, request.Password);
            
            await _repo.UpdateUserAsync(user);
            
            var accessToken = _tokenGenerator.GenerateAccessToken(user);
            var refreshToken = new RefreshToken
            {
                Token = _tokenGenerator.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = user.Id
            };
            
            await _repo.CreateRefreshTokenAsync(refreshToken);
    
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
            
            return Ok(response);
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
    /// Generate a new pair of tokens
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var currentRefreshToken = await _repo.GetRefreshTokenAsync(request.Token);
            
            if (currentRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                await _repo.RemoveRefreshTokenAsync(currentRefreshToken.Token);
                return Unauthorized("Expired refresh token. Please log in again.");
            }
            
            var newAccessToken = _tokenGenerator.GenerateAccessToken(currentRefreshToken.User!);
            var newRefreshToken = new RefreshToken
            {
                Token = _tokenGenerator.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = currentRefreshToken.UserId
            };
            
            await _repo.CreateRefreshTokenAsync(newRefreshToken);
            await _repo.RemoveRefreshTokenAsync(currentRefreshToken.Token);
            
            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
            
            return Ok(response);
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
    /// Clear all refresh tokens
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("logout/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout([FromRoute] int id)
    {
        try
        {
            await _repo.RemoveAllRefreshTokensAsync(id);
            return Ok("User logged out.");
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