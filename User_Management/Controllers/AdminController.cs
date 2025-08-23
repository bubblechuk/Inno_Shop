using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User_Management.Controllers.DTO;
using User_Management.Database;
using User_Management.Database.Models;
using User_Management.Services;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _repository;

    public AdminController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("add_user")]
    public async Task<IActionResult> AddUser([FromBody] UserAdd useradd)
    {
        var hasher = new PasswordHasher<User>();
        var user = new User()
        {
            Name = useradd.Name,
            Email = useradd.Email,
            Role = useradd.Role,
            IsActive = true
            
        };
        user.Password = hasher.HashPassword(user, useradd.Password);
        _repository.AddAsync(user);
        return Ok();
    }

    [HttpPut("update_user")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdate userupdate)
    {
        var hasher = new PasswordHasher<User>();
        var user = new User()
        {
            Id = userupdate.Id,
            Name = userupdate.Name,
            Email = userupdate.Email,
            Role = userupdate.Role,
            IsActive = userupdate.IsActive
        };
        user.Password = hasher.HashPassword(user, userupdate.Password);
        _repository.UpdateAsync(user);
        return Ok();
    }

    [HttpPatch("delete_user")]
    public async Task<IActionResult> DeleteUser([FromBody] int id)
    {
        await _repository.DeleteAsync(id);
        return Ok();
    }
    [HttpPatch("restore_user")]
    public async Task<IActionResult> RestoreUser([FromBody] int id)
    {
        await _repository.RestoreAsync(id);
        return Ok();
    }

    [HttpGet("get_users")]
    public async Task<IActionResult> getUsers()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }
}