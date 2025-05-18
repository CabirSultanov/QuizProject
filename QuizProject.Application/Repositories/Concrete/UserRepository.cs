using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using QuizProject.Application.Models;
using QuizProject.Application.Data;
using QuizProject.Application.Repositories.Abstract;
using QuizProject.Application.Cloudinary;

namespace QuizProject.Application.Repositories.Concrete;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMediaUpload _mediaUpload;

    public UserRepository(AppDbContext context, IMediaUpload mediaUpload)
    {
        _context = context;
        _mediaUpload = mediaUpload;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.Include(u => u.Role).ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        return user;
    }

    public async Task<User> UpdateUserAsync(User user, IFormFile? image)
    {
        if (image != null)
        {
            var uploadResult = await _mediaUpload.UploadImageAsync(image, "user");
            user.ImageUrl = uploadResult.SecureUrl.ToString();
        }
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
} 