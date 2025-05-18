using System.Threading.Tasks;

namespace QuizProject.Application.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
} 