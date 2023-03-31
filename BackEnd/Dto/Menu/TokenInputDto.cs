namespace SummerBootAdmin.Dto.Menu;

public class TokenInputDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class TokenOutputDto
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
