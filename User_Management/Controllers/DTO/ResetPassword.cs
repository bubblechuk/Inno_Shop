namespace User_Management.Controllers.DTO;

public class ResetPassword
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}