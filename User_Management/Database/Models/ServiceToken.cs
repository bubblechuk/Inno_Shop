using System.ComponentModel.DataAnnotations;

namespace User_Management.Database.Models;

public class ServiceToken
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public string Type { get; set; }
    public DateTime Expires { get; set; }
    public bool isUsed { get; set; }
}