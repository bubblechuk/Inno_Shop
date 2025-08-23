using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User_Management.Controllers.DTO;
using User_Management.Database.Models;

namespace User_Management.Database;

public interface IUserRepository
{
    Task<AuthResponse> Authenticate(Authentication authentication);
    Task<string> Register(Registration registration);
    Task<bool> ConfirmEmail(string token);
    Task<string> ForgotPassword(string email);
    Task<bool> ResetPassword(string token, string password);
    Task AddToken(ServiceToken token);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
    Task<List<User>> GetAllAsync();
}

public class UserRepository : IUserRepository
{
    private readonly Context _context;

    public UserRepository(Context context)
    {
        _context = context;
    }

    public async Task<AuthResponse> Authenticate(Authentication authentication)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == authentication.Email);
        var hasher = new PasswordHasher<User>();
        if (user != null)
        {
            
            var result = hasher.VerifyHashedPassword(user, 
                                        user.Password, 
                                        authentication.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return null;
            }
            else
            {
                return new AuthResponse()
                {
                    Email = user.Email,
                    Role = user.Role
                };
            }

        }
        else
        {
            return null;
        }
    }
    public async Task<string> Register(Registration registration)
    {
        if (_context.Users.Any(x => x.Email == registration.Email))
            return null;
        var hasher = new PasswordHasher<User>();
        var token = Guid.NewGuid().ToString();
        var user = new User()
        {
            Name = registration.Name,
            Email = registration.Email,
            Role = "User",
            IsActive = false
        };
        try
        {
            var hashedPassword = hasher.HashPassword(user, registration.Password);
            user.Password = hashedPassword;
            await AddAsync(user);
            var emailToken = new ServiceToken()
            {
                UserId = _context.Users.FirstOrDefault(x => x.Email == registration.Email).Id,
                Token = token,
                Type = "EmailConfirmation",
                Expires = DateTime.UtcNow.AddHours(1),
                isUsed = false
            };
            await AddToken(emailToken);
            return token;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return null;
        }
    }

    public async Task<bool> ConfirmEmail(string token)
    {
        var exisitingToken = await _context.Tokens.FirstOrDefaultAsync(x => x.Token == token);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == exisitingToken.UserId);
        if (user != null && exisitingToken != null)
        {
            user.IsActive = true;
            await UpdateAsync(user);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<string> ForgotPassword(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        var existingToken = await _context.Tokens.FirstOrDefaultAsync(x => x.Type == "PasswordReset" && x.Expires > DateTime.UtcNow);
        var token = Guid.NewGuid().ToString();
        if (user != null && existingToken == null)
        {
            ServiceToken servicetoken = new ServiceToken()
            {
                UserId = user.Id,
                Token = token,
                Type = "PasswordReset",
                Expires = DateTime.UtcNow.AddHours(1),
                isUsed = false,
                
            };
            await AddToken(servicetoken);
            return token;
        }
        else if (existingToken != null)
        {
            return existingToken.Token;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> ResetPassword(string token, string password)
    {
        var existingToken = await _context.Tokens.FirstOrDefaultAsync(x => x.Type == "PasswordReset" && x.Expires > DateTime.UtcNow);
        if (existingToken.Token == token)
        {
            var hasher = new PasswordHasher<User>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == existingToken.UserId);
            if (user != null)
            {
                var hashedPassword = hasher.HashPassword(user, password);
                user.Password = hashedPassword; 
                await UpdateAsync(user);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task AddToken(ServiceToken token)
    {
        await _context.Tokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
    public async Task RestoreAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<User>> GetAllAsync()
    {
        return _context.Users.ToList();
    }
}