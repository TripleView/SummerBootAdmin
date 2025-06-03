using Microsoft.AspNetCore.Mvc;
using SummerBoot.Core;
using SummerBootAdmin.Dto.Login;
using SummerBootAdmin.Service;

namespace SummerBootAdmin.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly ILoginService loginService;

    public LoginController(IConfiguration configuration, ILoginService loginService)
    {
        this.configuration = configuration;
        this.loginService = loginService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResult<LoginOutPutDto>> Token([FromBody] LoginInputDto dto)
    {
        var result = await loginService.Login(dto);

        return ApiResult<LoginOutPutDto>.Ok(result);
    }

    [HttpGet]
    public async Task<ApiResult<string>> Version()
    {
        return ApiResult<string>.Ok("1.6.9");
    }

}