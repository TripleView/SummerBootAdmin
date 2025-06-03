using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto.Login;
using SummerBootAdmin.Model.Role;
using SummerBootAdmin.Repository.User;

namespace SummerBootAdmin.Service;

public interface ILoginService
{
    Task<LoginOutPutDto> Login(LoginInputDto dto);
}

[AutoRegister(typeof(ILoginService))]
public class LoginService : ILoginService
{
    private readonly IConfiguration configuration;
    private readonly IUserRepository userRepository;
    private readonly IUserRoleRepository userRoleRepository;

    public LoginService(IConfiguration configuration, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
    {
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.userRoleRepository = userRoleRepository;
    }

    public async Task<LoginOutPutDto> Login(LoginInputDto dto)
    {
        var password = dto.Password;
        var user = await userRepository.FirstOrDefaultAsync(x => x.Account == dto.Account);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new Exception("用户名/密码错误，请确认");
        }

        var roles = await userRoleRepository.InnerJoin(new Role(), x => x.T1.RoleId == x.T2.Id).Where(x => x.T1.UserId == user.Id)
            .Select(x => x.T2).ToListAsync();

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
            new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddDays(3)).ToUnixTimeSeconds()}"),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim("account", user.Account),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }
        var jwtSecurityKey = configuration["jwtSecurityKey"];
        var jwtDomain = configuration["jwtDomain"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: jwtDomain,
            audience: jwtDomain,
            claims: claims,
            expires: DateTime.Now.AddDays(3),
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var result = new LoginOutPutDto()
        {
            Token = token,
            UserInfo = new UserInfo()
            {
                Dashboard = "0",
                Role = roles.Select(x => x.Name).ToList(),
                UserId = user.Id.ToString(),
                UserName = user.Account + $"({user.Name})"
            }
        };
        return result;
    }
}