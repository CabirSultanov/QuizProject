using FluentValidation;
using QuizProject.Contracts.Requests.Auth;
using QuizProject.Contracts.Requests;

namespace QuizProject.Application.Validators.Auth;

public class LoginRequestValidator  : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}