using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.ExpressionParser.Parser;
using SummerBootAdmin.Dto.Role;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Model;
using SummerBootAdmin.Model.Role;
using SummerBootAdmin.Repository;
using SummerBootAdmin.Repository.Department;
namespace SummerBootAdmin;

[ApiController]
[Route("api/[controller]/[action]")]
public class RoleController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IRoleRepository roleRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;

    public RoleController(IConfiguration configuration, IRoleRepository roleRepository, IUnitOfWork1 unitOfWork1, IMapper mapper)
    {
        this.configuration = configuration;
        this.roleRepository = roleRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
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

        await roleRepository.DeleteAsync(it => deleteMenusDto.Ids.Contains( it.Id));
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<Page<Role>>> GetRolesByPage([FromBody] PageQueryRoleDto dto)
    {
        var result= await roleRepository.ToPageAsync(dto);
        return ApiResult<Page<Role>>.Ok(result);
    }
    
    [HttpGet]
    public async Task<ApiResult<List<Role>>> List()
    {
        var roles = await roleRepository.ToListAsync();

        return ApiResult<List<Role>>.Ok(roles);
    }


}