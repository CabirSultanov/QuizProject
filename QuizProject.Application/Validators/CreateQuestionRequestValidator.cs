using FluentValidation;
using QuizProject.Contracts.Requests;

namespace QuizProject.Application.Validators;

public class CreateQuestionRequestValidator  : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Question text is required.");
        
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required.");
        
        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1).WithMessage("At least two answers are required.")
            .Must(x => x.Count(a => a.IsCorrect) == 1).WithMessage("Exactly one answer must be marked as correct.");
    }
}