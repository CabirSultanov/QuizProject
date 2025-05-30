using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QuizProject.Application.Cloudinary;
using QuizProject.Application.Data;
using QuizProject.Application.Helpers;
using QuizProject.Application.Models;
using QuizProject.Application.Repositories.Abstract;
using System;

namespace QuizProject.Application.Repositories.Concrete;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly IMediaUpload _mediaUpload;
    
    public AuthRepository(AppDbContext context, IMediaUpload mediaUpload)
    {
        _context = context;
        _mediaUpload = mediaUpload;
    }
    
    public async Task<User> RegisterUserAsync(User user, IFormFile? image)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            throw new ArgumentException("Username already exists.");
        }
        
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            throw new ArgumentException("Email already exists.");
        }
        
        if (await _context.Users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber))
        {
            throw new ArgumentException("Phone number already exists.");
        }
        
        user.Password = Hasher.HashPassword($"{user.Password}{user.Salt}");
        
        if (image is not null)
        {
            var uploadResult = await _mediaUpload.UploadImageAsync(image, "user");
            user.ImageUrl = uploadResult.SecureUrl.ToString();
        }
        
        user.EmailVerificationCode = new Random().Next(1000, 10000).ToString();
        user.EmailVerificationExpiry = DateTime.UtcNow.AddMinutes(15);
        user.IsEmailVerified = false;
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return user;
    }
    
    public async Task<User> LoginUserAsync(string username, string password)
    {
        var loggedInUser = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
        
        if (loggedInUser is null)
        {
            throw new KeyNotFoundException("Invalid username or password.");
        }
        
        var hashedPassword = Hasher.HashPassword($"{password}{loggedInUser.Salt}");
        if (hashedPassword != loggedInUser.Password)
        {
            throw new KeyNotFoundException("Invalid password.");
        }
        
        return loggedInUser;
    }
    
    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }
    
    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(r => r.User)
            .ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(t => t.Token == token);
    
        if (refreshToken is null)
        {
            throw new KeyNotFoundException("Invalid refresh token.");
        }
        return refreshToken;
    }
    
    public async Task RemoveRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (refreshToken is null)
        {
            throw new KeyNotFoundException("Invalid refresh token.");
        }
        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemoveAllRefreshTokensAsync(int userId)
    {
        var tokens = await _context.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();
        if (tokens is null || !tokens.Any())
        {
            throw new KeyNotFoundException("No refresh tokens found for the user.");
        }
        _context.RefreshTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }
    
    public async Task<User> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        
        return user;
    }
}