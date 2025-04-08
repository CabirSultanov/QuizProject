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
    private readonly IUserRepository _repo;
    private readonly ITokenGenerator _tokenGenerator;
    
    public AuthController(IUserRepository repo, ITokenGenerator tokenGenerator)
    {
        _repo = repo;
        _tokenGenerator = tokenGenerator;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        var user = request.MapToUser();
        try
        {
            var registeredUser = await _repo.RegisterUserAsync(user);
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
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        try
        {
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
    
    [HttpPost("refresh")]
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
    
    [HttpPost("logout/{id}")]
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