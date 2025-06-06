using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummerBoot.Cache;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto.Login;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Dto.Role;
using SummerBootAdmin.Model.Role;
using SummerBootAdmin.Repository;
using SummerBootAdmin.Repository.Menu;
using SummerBootAdmin.Repository.Role;

namespace SummerBootAdmin.Controllers;

[Authorize(Policy = "urlPolicy")]
[ApiController]
[Route("api/[controller]/[action]")]
public class RoleController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IRoleRepository roleRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;
    private readonly IRoleAssignMenuRepository roleAssignMenuRepository;
    private readonly IMenuRepository menuRepository;
    private readonly ICache cache;

    public RoleController(IConfiguration configuration, IRoleRepository roleRepository, IUnitOfWork1 unitOfWork1, IMapper mapper, IRoleAssignMenuRepository roleAssignMenuRepository, IMenuRepository menuRepository, ICache cache)
    {
        this.configuration = configuration;
        this.roleRepository = roleRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
        this.roleAssignMenuRepository = roleAssignMenuRepository;
        this.menuRepository = menuRepository;
        this.cache = cache;
    }

    [HttpPost]
    public async Task<ApiResult<Role>> AddRole([FromBody] AddRoleDto dto)
    {
        var role = mapper.Map<AddRoleDto, Role>(dto);
        await CheckRole(role, true);
        unitOfWork1.BeginTransaction();
        var dbRole = await roleRepository.InsertAsync(role);

        unitOfWork1.Commit();
        return ApiResult<Role>.Ok(dbRole);
    }


    [HttpPost]
    public async Task<ApiResult<Role>> UpdateRole([FromBody] Role role)
    {
        unitOfWork1.BeginTransaction();
        var dbRole = await roleRepository.GetAsync(role.Id);
        if (dbRole == null)
        {
            throw new Exception("要修改的角色不存在");
        }

        if (dbRole.Name == "admin")
        {
            throw new Exception("系统角色无法修改");
        }

        await CheckRole(role, true);

        await roleRepository.UpdateAsync(role);

        unitOfWork1.Commit();
        return ApiResult<Role>.Ok(dbRole);
    }


    private async Task<bool> CheckRole(Role role, bool isUpdate)
    {
        var query = QueryCondition.True<Role>();
        query = query.And(it => it.Name == role.Name);
        if (isUpdate)
        {
            query = query.And(it => it.Id != role.Id);
        }
        var dbRole = await roleRepository.FirstOrDefaultAsync(query);
        if (dbRole != null)
        {
            throw new Exception("相同名称的角色已存在");
        }

        return true;
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteRoles([FromBody] DeleteMenusDto deleteMenusDto)
    {
        if (deleteMenusDto.Ids == null || deleteMenusDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        await roleRepository.DeleteAsync(it => deleteMenusDto.Ids.Contains(it.Id));
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<Page<Role>>> GetRolesByPage([FromBody] PageQueryRoleDto dto)
    {
        var result = await roleRepository.ToPageAsync(dto);
        return ApiResult<Page<Role>>.Ok(result);
    }

    [HttpGet]
    public async Task<ApiResult<List<Role>>> List()
    {
        var roles = await roleRepository.ToListAsync();

        return ApiResult<List<Role>>.Ok(roles);
    }

    /// <summary>
    /// 给角色分配权限
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost]
    public async Task<ApiResult<bool>> RoleAssignPermissions([FromBody] RoleAssignPermissionsDto dto)
    {
        if (dto.RoleIds == null || dto.RoleIds.Count == 0 || dto.MenuIds == null || dto.MenuIds.Count == 0)
        {
            throw new Exception("参数不正确");
        }

        var adminRole = await roleRepository.FirstOrDefaultAsync(x => dto.RoleIds.Contains(x.Id) && x.Name == "admin");
        if (adminRole != null)
        {
            throw new Exception("admin为系统角色，无法修改");
        }

        unitOfWork1.BeginTransaction();
        foreach (var dtoRoleId in dto.RoleIds)
        {
            var dbRole = await roleRepository.GetAsync(dtoRoleId);
            if (dbRole == null)
            {
                throw new Exception("角色不存在");
            }

            await roleAssignMenuRepository.DeleteAsync(it => it.RoleId == dtoRoleId);

            var roleAssignMenus = dto.MenuIds.Select(it => new RoleAssignMenu()
            {
                RoleId = dtoRoleId,
                MenuId = it
            }).ToList();

            await roleAssignMenuRepository.InsertAsync(roleAssignMenus);
        }

        unitOfWork1.Commit();
        await cache.RemoveAsync(LoginConst.RoleApisKey);
        return ApiResult<bool>.Ok(true);
    }

    /// <summary>
    /// 查看角色拥有的权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResult<GetRolePermissionsOutputDto>> GetRolePermissions([FromQuery] int roleId)
    {
        var dbRole = await roleRepository.GetAsync(roleId);
        if (dbRole == null)
        {
            throw new Exception("角色不存在");
        }

        var menuIds = new List<int>();
        if (dbRole.Name == "admin")
        {
            menuIds = (await menuRepository.GetAllAsync()).Select(x => x.Id).ToList();
        }
        else
        {
            menuIds = await roleAssignMenuRepository.Where(x => x.RoleId == roleId).Select(x => x.MenuId).ToListAsync();
        }

        var result = new GetRolePermissionsOutputDto()
        {
            MenuIds = menuIds
        };
        return ApiResult<GetRolePermissionsOutputDto>.Ok(result);
    }
}