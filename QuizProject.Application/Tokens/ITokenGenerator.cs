using QuizProject.Application.Models;

namespace QuizProject.Application.Tokens;

public interface ITokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}