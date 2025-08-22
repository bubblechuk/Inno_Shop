using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace User_Management.Database.Models;

[Index(nameof(Email), IsUnique=true)]
public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    [MaxLength(128)]
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}