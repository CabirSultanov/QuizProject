using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using QuizProject.Application.Models;

namespace QuizProject.Application.Tokens;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtOptions _jwtOptions;
    
    public TokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    
    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new("role", user.Role!.Name),
        };
        
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(_jwtOptions.AccessValidFor),
            signingCredentials: _jwtOptions.SigningCredentials,
            claims: claims
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken() => Guid.NewGuid().ToString();
}