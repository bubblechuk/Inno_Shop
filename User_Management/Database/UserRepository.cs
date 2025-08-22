using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User_Management.Controllers.DTO;
using User_Management.Database.Models;

namespace User_Management.Database;

public interface IUserRepository
{
    Task<AuthResponse> Authenticate(Authentication authentication);
    Task<bool> Register(Registration registration);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
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
    public async Task<bool> Register(Registration registration)
    {
        if (_context.Users.Any(x => x.Email == registration.Email))
            return false;
        var hasher = new PasswordHasher<User>();
        var user = new User()
        {
            Name = registration.Name,
            Email = registration.Email,
            Role = "User",
            IsActive = true
        };
        try
        {
            var hashedPassword = hasher.HashPassword(user, registration.Password);
            user.Password = hashedPassword;
            await AddAsync(user);
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }
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

    public async Task<List<User>> GetAllAsync()
    {
        return _context.Users.ToList();
    }
}