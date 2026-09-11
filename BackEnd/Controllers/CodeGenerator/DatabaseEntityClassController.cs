using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlParser.Net.Ast;
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
public class DatabaseEntityClassController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IDatabaseEntityClassRepository databaseEntityClassRepository;
    private readonly IDatabaseEntityFieldRepository databaseEntityFieldRepository;
    private readonly IDatabaseEntityFieldTypeRepository databaseEntityFieldTypeRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;
    private readonly ICache cache;

    public DatabaseEntityClassController(IConfiguration configuration, IDatabaseEntityClassRepository databaseEntityClassRepository, IDatabaseEntityFieldRepository databaseEntityFieldRepository, IDatabaseEntityFieldTypeRepository databaseEntityFieldTypeRepository, IUnitOfWork1 unitOfWork1, IMapper mapper, ICache cache)
    {
        this.configuration = configuration;
        this.databaseEntityClassRepository = databaseEntityClassRepository;
        this.databaseEntityFieldRepository = databaseEntityFieldRepository;
        this.databaseEntityFieldTypeRepository = databaseEntityFieldTypeRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
        this.cache = cache;
    }

    [HttpPost]
    public async Task<ApiResult<DatabaseEntityClass>> CreateDatabaseEntityClass([FromBody] DatabaseEntityClass dto)
    {
        await CheckDatabaseEntityClass(dto, false);
        unitOfWork1.BeginTransaction();
        var dbDatabaseEntityClass = await databaseEntityClassRepository.InsertAsync(dto);
        var i = 0;
        dto.Fields.ForEach(x =>
        {
            x.ClassId = dto.Id;
            x.OrderIndex = i++;
        });

        await databaseEntityFieldRepository.FastBatchInsertAsync(dto.Fields);

        unitOfWork1.Commit();
        return ApiResult<DatabaseEntityClass>.Ok(dbDatabaseEntityClass);
    }


    [HttpPost]
    public async Task<ApiResult<DatabaseEntityClass>> UpdateDatabaseEntityClass([FromBody] DatabaseEntityClass databaseEntityClass)
    {
        unitOfWork1.BeginTransaction();
        var dbDatabaseEntityClass = await databaseEntityClassRepository.GetAsync(databaseEntityClass.Id);
        if (dbDatabaseEntityClass == null)
        {
            throw new Exception("要修改的字段类型不存在");
        }

        await CheckDatabaseEntityClass(databaseEntityClass, true);

        await databaseEntityClassRepository.UpdateAsync(databaseEntityClass);
        await databaseEntityFieldRepository.DeleteAsync(x => x.ClassId == databaseEntityClass.Id);
        var i = 0;
        databaseEntityClass.Fields.ForEach(x =>
        {
            x.ClassId = databaseEntityClass.Id;
            x.OrderIndex = i++;
        });

        await databaseEntityFieldRepository.FastBatchInsertAsync(databaseEntityClass.Fields);
        unitOfWork1.Commit();
        return ApiResult<DatabaseEntityClass>.Ok(dbDatabaseEntityClass);
    }


    private async Task<bool> CheckDatabaseEntityClass(DatabaseEntityClass databaseEntityClass, bool isUpdate)
    {
        if (databaseEntityClass.Fields.IsNullOrEmpty())
        {
            throw new Exception("类的字段列表不能为空");
        }
        var query = QueryCondition.True<DatabaseEntityClass>();
        query = query.And(it => it.Name == databaseEntityClass.Name);
        if (isUpdate)
        {
            query = query.And(it => it.Id != databaseEntityClass.Id);
        }
        var dbDatabaseEntityClass = await databaseEntityClassRepository.FirstOrDefaultAsync(query);
        if (dbDatabaseEntityClass != null)
        {
            throw new Exception("相同名称的类型已存在");
        }

        return true;
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteDatabaseEntityClasss([FromBody] DeleteByIdsDto deleteByIdsDto)
    {
        if (deleteByIdsDto.Ids == null || deleteByIdsDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        await databaseEntityClassRepository.DeleteAsync(it => deleteByIdsDto.Ids.Contains(it.Id));
        await databaseEntityFieldRepository.DeleteAsync(it => deleteByIdsDto.Ids.Contains(it.ClassId));
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<Page<DatabaseEntityClass>>> GetDatabaseEntityClasssByPage([FromBody] PageQueryDatabaseEntityClassDto dto)
    {
        var condition = QueryCondition.True<DatabaseEntityClass>();
        if (dto.Name.HasText())
        {
            condition = condition.And<DatabaseEntityClass>(x => x.Name.Contains(dto.Name));
        }
        if (dto.Description.HasText())
        {
            condition = condition.And<DatabaseEntityClass>(x => x.Description.Contains(dto.Description));
        }
        var result = await databaseEntityClassRepository.Where(condition).ToPageAsync(dto);
        return ApiResult<Page<DatabaseEntityClass>>.Ok(result);
    }

    [HttpGet]
    public async Task<ApiResult<List<DatabaseEntityClass>>> List()
    {
        var databaseEntityClasses = await databaseEntityClassRepository.ToListAsync();

        return ApiResult<List<DatabaseEntityClass>>.Ok(databaseEntityClasses);
    }

    [HttpGet]
    public async Task<ApiResult<List<DatabaseEntityField>>> GetDatabaseEntityFields(int classId)
    {
        var databaseEntityClasses = await databaseEntityFieldRepository.Where(x => x.ClassId == classId).OrderBy(x => x.OrderIndex).ToListAsync();

        return ApiResult<List<DatabaseEntityField>>.Ok(databaseEntityClasses);
    }

    [HttpPost]
    public async Task<ApiResult<bool>> GenerateModel([FromBody] IdsDto dto)
    {

        var databaseEntityClasses =
            await databaseEntityClassRepository.Where(x => dto.Ids.Contains(x.Id)).ToListAsync();
        var databaseEntityFields =
            await databaseEntityFieldRepository.Where(x => dto.Ids.Contains(x.ClassId)).ToListAsync();

        var fieldTypes = await databaseEntityFieldTypeRepository
            .Where(x => databaseEntityFields.Select(y => y.FieldTypeId).Distinct().Contains(x.Id))
            .ToListAsync();

        foreach (var databaseEntityClass in databaseEntityClasses)
        {
            var fields = databaseEntityFields.Where(x => x.ClassId == databaseEntityClass.Id).OrderBy(x => x.OrderIndex)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine("using SummerBoot.Repository;");
            sb.AppendLine("namespace SummerBootAdmin.Model.User;");
            if (databaseEntityClass.Description.HasText())
            {
                sb.AppendLine($"/// <summary>");
                sb.AppendLine($"/// {databaseEntityClass.Description}");
                sb.AppendLine($"/// </summary>");
                sb.AppendLine($"[Description(\"{databaseEntityClass.Description}\")]");
            }
            sb.AppendLine($"public class {databaseEntityClass.Name} : BaseEntity");
            sb.AppendLine($"{{");
            foreach (var field in fields)
            {
                var fieldType = fieldTypes.First(x => x.Id == field.FieldTypeId);
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {field.Description}");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    [Description(\"{field.Description}\")]");
                sb.AppendLine($"    public {fieldType.Value} {field.Name} {{ get; set; }}");
            }
            sb.AppendLine($"}}");
            var modelTxt = sb.ToString();

        }



        return ApiResult<bool>.Ok(true);
    }
}