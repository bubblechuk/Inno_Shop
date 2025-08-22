using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using User_Management.Controllers.DTO;
using User_Management.Database;
using User_Management.Database.Models;
using User_Management.Services;

namespace User_Management.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly JWTService _jwtService;

    public UserController(IUserRepository repository, JWTService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    [HttpPost("authentication")]
    public async Task<IActionResult> Authenticate([FromBody] Authentication authentication)
    {
        var authResponse = await _repository.Authenticate(authentication);
        if (authResponse != null)
        {
            var token = _jwtService.GenerateJWTToken(authResponse.Email,
                                      authResponse.Role);
            return Ok(token);
        }
        else
        {
            return StatusCode(401, "Неверная почта или пароль");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> register([FromBody] Registration registration)
    {
        if (await _repository.Register(registration))
        {
            return Ok("Вы успешно зарегистрировались!");
        }
        else
        {
            return StatusCode(500, "Аккаунт с такой почтой уже сущесвует либо сервер недоступен.");
        }
    }
}