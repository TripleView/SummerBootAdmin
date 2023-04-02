using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.ExpressionParser.Parser;
using SummerBootAdmin.Dto.Menu;

namespace SummerBootAdmin;

[ApiController]
[Route("api/[controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration configuration;

    public LoginController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }


    [HttpPost]
    public async Task<ApiResult<TokenOutputDto>> Token([FromBody] TokenInputDto dto)
    {
        if (dto.UserName == "admin"&&dto.Password== "21232f297a57a5a743894a0e4a801fc3")
        {
            var result = new TokenOutputDto()
            {
                Token = "SCUI.Administrator.Auth",
                UserInfo = new UserInfo()
                {
                    Dashboard = "0",
                    Role = new List<string>() { "SA", "admin", "Auditor" },
                    UserId = "1",
                    UserName = "admin"
                }
            };
            return ApiResult<TokenOutputDto>.Ok(result);
        }
      
        return ApiResult<TokenOutputDto>.Ng("登录失败");
    }

    [HttpGet]
    public async Task<ApiResult<string>> Version()
    {
        return ApiResult<string>.Ok("1.6.9");
    }

}