using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Dto.User;
using SummerBootAdmin.Model.User;
using SummerBootAdmin.Repository.Department;
namespace SummerBootAdmin;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;
    private readonly IRoleRepository roleRepository;

    public UserController(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork1 unitOfWork1, IMapper mapper, IRoleRepository roleRepository)
    {
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
        this.roleRepository = roleRepository;
    }

    [HttpPost]
    public async Task<ApiResult<User>> AddUser([FromBody] AddUserDto dto)
    {
        var user = mapper.Map<AddUserDto, User>(dto);
        await CheckUser(user, true);
        unitOfWork1.BeginTransaction();
        var dbUser = await userRepository.InsertAsync(user);

        unitOfWork1.Commit();
        return ApiResult<User>.Ok(dbUser);
    }


    [HttpPost]
    public async Task<ApiResult<User>> UpdateUser([FromBody] User user)
    {
        unitOfWork1.BeginTransaction();
        var dbUser = await userRepository.GetAsync(user.Id);
        if (dbUser == null)
        {
            throw new Exception("要修改的用户不存在");
        }

        await CheckUser(user, true);

        await userRepository.UpdateAsync(user);

        unitOfWork1.Commit();
        return ApiResult<User>.Ok(dbUser);
    }


    private async Task<bool> CheckUser(User user, bool isUpdate)
    {
        var query = QueryCondition.True<User>();
        query = query.And(it => it.Account == user.Account);
        if (isUpdate)
        {
            query = query.And(it => it.Id != user.Id);
        }
        var dbUser = await userRepository.FirstOrDefaultAsync(query);
        if (dbUser != null)
        {
            throw new Exception("相同账号的用户已存在");
        }

        return true;
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteUsers([FromBody] DeleteMenusDto deleteMenusDto)
    {
        if (deleteMenusDto.Ids == null || deleteMenusDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        await userRepository.DeleteAsync(it => deleteMenusDto.Ids.Contains(it.Id));
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<Page<User>>> GetUsersByPage([FromBody] PageQueryUserDto dto)
    {
        var condition = QueryCondition.True<User>();
        if (dto.DepartmentId.HasValue)
        {
            condition = condition.And(it => it.DepartmentId == dto.DepartmentId.Value);
        }
        var result = await userRepository.Where(condition).ToPageAsync(dto);
        if (result.Data.Count > 0)
        {
            var roleIds = result.Data.Select(it => it.RoleId).ToList();
            var roles = await roleRepository.Where(it => roleIds.Contains(it.Id)).ToListAsync();
            foreach (var user in result.Data)
            {
                user.RoleName = roles.FirstOrDefault(it => it.Id == user.RoleId)?.Name ?? "";
            }
        }
        return ApiResult<Page<User>>.Ok(result);
    }

    [HttpGet]
    public async Task<ApiResult<List<User>>> List()
    {
        var users = await userRepository.ToListAsync();

        return ApiResult<List<User>>.Ok(users);
    }


}