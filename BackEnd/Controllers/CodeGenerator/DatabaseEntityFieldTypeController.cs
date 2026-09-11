using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummerBoot.Cache;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto;
using SummerBootAdmin.Dto.CodeGenerator;
using SummerBootAdmin.Dto.Dictionary;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Model.CodeGenerator;
using SummerBootAdmin.Repository.CodeGenerator;

namespace SummerBootAdmin.Controllers.CodeGenerator;

[Authorize(Policy = "urlPolicy")]
[ApiController]
[Route("api/[controller]/[action]")]
public class DatabaseEntityFieldTypeController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IDatabaseEntityFieldTypeRepository databaseEntityFieldTypeRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;
    private readonly ICache cache;

    public DatabaseEntityFieldTypeController(IConfiguration configuration, IDatabaseEntityFieldTypeRepository databaseEntityFieldTypeRepository, IUnitOfWork1 unitOfWork1, IMapper mapper, ICache cache)
    {
        this.configuration = configuration;
        this.databaseEntityFieldTypeRepository = databaseEntityFieldTypeRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
        this.cache = cache;
    }

    [HttpPost]
    public async Task<ApiResult<DatabaseEntityFieldType>> CreateDatabaseEntityFieldType([FromBody] DatabaseEntityFieldType dto)
    {
        await CheckDatabaseEntityFieldType(dto, false);
        unitOfWork1.BeginTransaction();
        var dbDatabaseEntityFieldType = await databaseEntityFieldTypeRepository.InsertAsync(dto);

        unitOfWork1.Commit();
        return ApiResult<DatabaseEntityFieldType>.Ok(dbDatabaseEntityFieldType);
    }


    [HttpPost]
    public async Task<ApiResult<DatabaseEntityFieldType>> UpdateDatabaseEntityFieldType([FromBody] DatabaseEntityFieldType databaseEntityFieldType)
    {
        unitOfWork1.BeginTransaction();
        var dbDatabaseEntityFieldType = await databaseEntityFieldTypeRepository.GetAsync(databaseEntityFieldType.Id);
        if (dbDatabaseEntityFieldType == null)
        {
            throw new Exception("要修改的字段类型不存在");
        }

        await CheckDatabaseEntityFieldType(databaseEntityFieldType, true);

        await databaseEntityFieldTypeRepository.UpdateAsync(databaseEntityFieldType);

        unitOfWork1.Commit();
        return ApiResult<DatabaseEntityFieldType>.Ok(dbDatabaseEntityFieldType);
    }


    private async Task<bool> CheckDatabaseEntityFieldType(DatabaseEntityFieldType databaseEntityFieldType, bool isUpdate)
    {
        var query = QueryCondition.True<DatabaseEntityFieldType>();
        query = query.And(it => it.Value == databaseEntityFieldType.Value);
        if (isUpdate)
        {
            query = query.And(it => it.Id != databaseEntityFieldType.Id);
        }
        var dbDatabaseEntityFieldType = await databaseEntityFieldTypeRepository.FirstOrDefaultAsync(query);
        if (dbDatabaseEntityFieldType != null)
        {
            throw new Exception("相同值的字段类型已存在");
        }

        return true;
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteDatabaseEntityFieldTypes([FromBody] DeleteByIdsDto deleteByIdsDto)
    {
        if (deleteByIdsDto.Ids == null || deleteByIdsDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        await databaseEntityFieldTypeRepository.DeleteAsync(it => deleteByIdsDto.Ids.Contains(it.Id));
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<Page<DatabaseEntityFieldType>>> GetDatabaseEntityFieldTypesByPage([FromBody] PageQueryDatabaseEntityFieldTypeDto dto)
    {
        var condition = QueryCondition.True<DatabaseEntityFieldType>();
        if (dto.Name.HasText())
        {
            condition = condition.And<DatabaseEntityFieldType>(x => x.Name.Contains(dto.Name));
        }
        if (dto.Value.HasText())
        {
            condition = condition.And<DatabaseEntityFieldType>(x => x.Value.Contains(dto.Value));
        }
        if (dto.Description.HasText())
        {
            condition = condition.And<DatabaseEntityFieldType>(x => x.Description.Contains(dto.Description));
        }
        var result = await databaseEntityFieldTypeRepository.Where(condition).OrderBy(x=>x.OrderIndex).ToPageAsync(dto);
        return ApiResult<Page<DatabaseEntityFieldType>>.Ok(result);
    }

    [HttpGet]
    public async Task<ApiResult<List<DatabaseEntityFieldType>>> List()
    {
        var databaseEntityFieldTypes = await databaseEntityFieldTypeRepository.OrderBy(x => x.OrderIndex).ToListAsync();

        return ApiResult<List<DatabaseEntityFieldType>>.Ok(databaseEntityFieldTypes);
    }

   
}