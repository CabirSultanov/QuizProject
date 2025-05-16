using FluentValidation;
using QuizProject.Contracts.Requests;

namespace QuizProject.Application.Validators;

public class QuizQuestionRequestValidator : AbstractValidator<QuizQuestionRequest>
{
    public QuizQuestionRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Question text is required.");
        
        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("At least one answer is required.")
            .Must(x => x.Count >= 2).WithMessage("At least two answers are required.")
            .Must(x => x.Count(a => a.IsCorrect) == 1).WithMessage("Exactly one answer must be marked as correct.");
        
        RuleForEach(x => x.Answers).SetValidator(new CreateAnswerRequestValidator());
    }
}