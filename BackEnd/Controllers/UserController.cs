using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Dto.User;
using SummerBootAdmin.Model.User;
using SummerBootAdmin.Repository.Role;
using SummerBootAdmin.Repository.User;

namespace SummerBootAdmin.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;
    private readonly IRoleRepository roleRepository;
    private readonly IUserRoleRepository userRoleRepository;

    public UserController(IConfiguration configuration, IUserRepository userRepository, IUnitOfWork1 unitOfWork1, IMapper mapper, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
    {
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
        this.roleRepository = roleRepository;
        this.userRoleRepository = userRoleRepository;
    }

    [HttpPost]
    public async Task<ApiResult<User>> AddUser([FromBody] AddUserDto dto)
    {
        var user = mapper.Map<AddUserDto, User>(dto);
        await CheckUser(user, true);
        unitOfWork1.BeginTransaction();
        user.Password = HashPassword(dto.Password);
        var dbUser = await userRepository.InsertAsync(user);
        await AddUserRoles(user);
        unitOfWork1.Commit();
        return ApiResult<User>.Ok(dbUser);
    }

    private async Task AddUserRoles(User user)
    {
        if (user.RoleIds?.Count > 0)
        {
            foreach (var roleId in user.RoleIds)
            {
                var userRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
                await userRoleRepository.InsertAsync(userRole);
            }
        }
    }

    // 哈希密码
    private string HashPassword(string password)
    {
        // 自动生成盐并哈希
        return BCrypt.Net.BCrypt.HashPassword(password);
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

        if (dbUser.Account == "admin")
        {
            throw new Exception("amdin用户无法编辑");
        }
        await CheckUser(user, true);
        user.Password = dbUser.Password;
        await userRepository.UpdateAsync(user);
        await userRoleRepository.DeleteAsync(it => it.UserId == user.Id);
        await AddUserRoles(user);
        unitOfWork1.Commit();
        return ApiResult<User>.Ok(dbUser);
    }

    [HttpPost]
    public async Task<ApiResult<bool>> AssignRoles([FromBody] AssignRolesDto dto)
    {
        if (dto.RoleIds == null || dto.RoleIds.Count == 0)
        {
            throw new Exception("请选择角色");
        }
        if (dto.UserIds == null || dto.UserIds.Count == 0)
        {
            throw new Exception("请选择用户");
        }
        unitOfWork1.BeginTransaction();
        var adminUserId = await userRepository.Where(x => x.Account == "admin").Select(x => x.Id).FirstOrDefaultAsync();
        if (dto.UserIds.Contains(adminUserId))
        {
            throw new Exception("无法编辑admin角色");
        }

        await userRoleRepository.DeleteAsync(x => dto.UserIds.Contains(x.UserId));

        foreach (var userId in dto.UserIds)
        {
            foreach (var roleId in dto.RoleIds)
            {
                var userRole = new UserRole()
                {
                    UserId = userId,
                    RoleId = roleId
                };
                await userRoleRepository.InsertAsync(userRole);
            }
        }
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<bool>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (dto.UserIds == null || dto.UserIds.Count == 0)
        {
            throw new Exception("请选择用户");
        }

        if (dto.Password.IsNullOrWhiteSpace())
        {
            throw new Exception("密码不能为空");
        }
        unitOfWork1.BeginTransaction();

        var users = await userRepository.Where(x => dto.UserIds.Contains(x.Id)).ToListAsync();
        foreach (var user in users)
        {
            user.Password = HashPassword(dto.Password);
            await userRepository.UpdateAsync(user);
        }
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
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

        var adminUserId = await userRepository.Where(x => x.Account == "admin").Select(x => x.Id).FirstOrDefaultAsync();
        if (deleteMenusDto.Ids.Contains(adminUserId))
        {
            throw new Exception("无法删除admin角色");
        }
        unitOfWork1.BeginTransaction();

        await userRepository.DeleteAsync(it => deleteMenusDto.Ids.Contains(it.Id));
        await userRoleRepository.DeleteAsync(it => deleteMenusDto.Ids.Contains(it.UserId));
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
        if (dto.Name.HasText())
        {
            condition = condition.And(it => it.Name.Contains(dto.Name));
        }
        var result = await userRepository.Where(condition).ToPageAsync(dto);
        if (result.Data.Count > 0)
        {
            var userIds = result.Data.Select(it => it.Id).ToList();
            var userRoles = await userRoleRepository.Where(x => userIds.Contains(x.UserId)).ToListAsync();
            var roles = await roleRepository.GetAllAsync();

            foreach (var user in result.Data)
            {
                var roleIds = userRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToList();
                var tempRoles = roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToList();
                user.RoleName = tempRoles.StringJoin(";");
                user.RoleIds = roleIds;
                user.Password = "";
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