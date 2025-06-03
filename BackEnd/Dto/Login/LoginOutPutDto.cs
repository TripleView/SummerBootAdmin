namespace SummerBootAdmin.Dto.Login;

public class LoginOutPutDto
{
    public string Token { get; set; }
    public UserInfo UserInfo { get; set; }
}

public class UserInfo
{
    public string Dashboard { get; set; }
    public List<string> Role { get; set; }
    public string UserId { get; set; }

    public string UserName { get; set; }
}