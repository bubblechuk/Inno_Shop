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
    public async Task<IActionResult> Register([FromBody] Registration registration)
    {
        var registerResponse = await _repository.Register(registration);
        if ( registerResponse != null)
        {
            return Ok(registerResponse);
        }
        else
        {
            return StatusCode(500, "Аккаунт с такой почтой уже сущесвует либо сервер недоступен.");
        }
    }

    [HttpPatch("email_confirmation")]
    public async Task<IActionResult> ConfirmEmail([FromBody] string token)
    {
        if (await _repository.ConfirmEmail(token))
        {
            return Ok("Почта подтверждена!");
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpPost("forgot_password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var token = await _repository.ForgotPassword(email);
        if (token != null)
        {
            return Ok(token);
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpPatch("reset_password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPassword resetPassword)
    {
        if (await _repository.ResetPassword(resetPassword.Token, resetPassword.NewPassword))
        {
            return Ok("Пароль изменен!");
        }
        else
        {
            return StatusCode(500, "Неверный токен либо сервер недоступен.");
        }
    }
}