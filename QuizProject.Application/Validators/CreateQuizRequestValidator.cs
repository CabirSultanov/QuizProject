using FluentValidation;
using QuizProject.Contracts.Requests;

namespace QuizProject.Application.Validators;

public class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequest>
{
    public CreateQuizRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");
        
        RuleFor(x => x.DifficultyLevel)
            .InclusiveBetween(1, 3).WithMessage("Difficulty level must be between 1 and 3.");
        
        RuleForEach(x => x.Questions)
            .SetValidator(new QuizQuestionRequestValidator());
    }
}