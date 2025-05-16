using QuizProject.Application.Models;
using QuizProject.Contracts.Requests;

namespace QuizProject.API.Mapping;

public static class ContractMapping
{
    public static User MapToUser(this RegisterRequest request)
    {
        return new User
        {
            Username = request.Username,
            Password = request.Password,
            Salt = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = "+994" + request.PhoneNumber,
            RoleId = request.RoleId,
        };
    }
}