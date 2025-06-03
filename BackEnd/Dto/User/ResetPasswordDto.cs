namespace SummerBootAdmin.Dto.User;

public class ResetPasswordDto
{
    public List<int> UserIds { get; set; }
    public string Password { get; set; }
}