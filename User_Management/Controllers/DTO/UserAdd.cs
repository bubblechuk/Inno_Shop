using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace User_Management.Database.Models;

public class UserAdd
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}