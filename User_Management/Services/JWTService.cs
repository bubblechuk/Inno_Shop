using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace User_Management.Services;

public class JWTService
{
    public JWTService()
    {
    }

    public string GenerateJWTToken(string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KEY")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("ISSUER"),
            audience: Environment.GetEnvironmentVariable("AUDIENCE"),
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}