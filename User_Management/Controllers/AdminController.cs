using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using User_Management.Controllers.DTO;
using User_Management.Database;
using User_Management.Database.Models;
using User_Management.Services;

[Route("api/[controller]")]
[Authorize(Roles = "User")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _repository;

    public AdminController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get_users")]
    public async Task<IActionResult> getUsers()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }
}