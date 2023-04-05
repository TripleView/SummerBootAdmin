using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.ExpressionParser.Parser;
using SummerBootAdmin.Dto.Department;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Model;
using SummerBootAdmin.Model.Department;
using SummerBootAdmin.Repository;
using SummerBootAdmin.Repository.Department;

namespace SummerBootAdmin;

[ApiController]
[Route("api/[controller]/[action]")]
public class DepartmentController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IDepartmentRepository departmentRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;

    public DepartmentController(IConfiguration configuration, IDepartmentRepository departmentRepository, IUnitOfWork1 unitOfWork1, IMapper mapper)
    {
        this.configuration = configuration;
        this.departmentRepository = departmentRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
    }

    [HttpPost]
    public async Task<ApiResult<Department>> AddDepartment([FromBody] AddDepartmentDto dto)
    {
        var department = mapper.Map<AddDepartmentDto, Department>(dto);
        await CheckDepartment(department, true);
        unitOfWork1.BeginTransaction();
        var dbDepartment = await departmentRepository.InsertAsync(department);

        unitOfWork1.Commit();
        return ApiResult<Department>.Ok(dbDepartment);
    }


    [HttpPost]
    public async Task<ApiResult<Department>> UpdateDepartment([FromBody] Department department)
    {
        unitOfWork1.BeginTransaction();
        var dbDepartment = await departmentRepository.GetAsync(department.Id);
        if (dbDepartment == null)
        {
            throw new Exception("要修改的字典不存在");
        }

        await CheckDepartment(department, true);

        await departmentRepository.UpdateAsync(department);

        unitOfWork1.Commit();
        return ApiResult<Department>.Ok(dbDepartment);
    }

    
    private async Task<bool> CheckDepartment(Department department, bool isUpdate)
    {
        var query = QueryCondition.True<Department>();
        query = query.And(it => it.Name == department.Name);
        if (isUpdate)
        {
            query = query.And(it => it.Id != department.Id);
        }
        var dbDepartment = await departmentRepository.FirstOrDefaultAsync(query);
        if (dbDepartment != null)
        {
            throw new Exception("相同名称的部门已存在");
        }

        return true;
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteDepartments([FromBody] DeleteMenusDto deleteMenusDto)
    {
        if (deleteMenusDto.Ids == null || deleteMenusDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        foreach (var id in deleteMenusDto.Ids)
        {
            await DeleteDepartmentsByRecursion(id);
        }
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

  

    private async Task<bool> DeleteDepartmentsByRecursion(int DepartmentId)
    {
        await departmentRepository.DeleteAsync(it => it.Id == DepartmentId);

        var childrens = await departmentRepository.Where(it => it.ParentId == DepartmentId).ToListAsync();
        if (childrens.Count == 0)
        {
            return true;
        }

        foreach (var childrenDepartment in childrens)
        {
            await DeleteDepartmentsByRecursion(childrenDepartment.Id);
        }

        return true;
    }

    [HttpGet]
    public async Task<ApiResult<List<Department>>> List()
    {
        var departments = await departmentRepository.ToListAsync();

        var result = AddDepartmentsTrees(departments, null);
        return ApiResult<List<Department>>.Ok(result);
    }



    private List<Department> AddDepartmentsTrees(List<Department> departments, int? parentId)
    {
        var list = departments.Where(it => it.ParentId == parentId).ToList();
        foreach (var Department in list)
        {
            var childrens = AddDepartmentsTrees(departments, Department.Id);
            if (Department.Children == null)
            {
                Department.Children = new List<Department>();
            }
            Department.Children.AddRange(childrens);
        }

        return list;
    }


}