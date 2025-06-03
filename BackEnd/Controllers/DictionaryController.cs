using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Dto.Dictionary;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Model.Dictionary;
using SummerBootAdmin.Repository.Dictionary;

namespace SummerBootAdmin.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class DictionaryController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IDictionaryRepository dictionaryRepository;
    private readonly IUnitOfWork1 unitOfWork1;
    private readonly IMapper mapper;
    private readonly IDictionaryItemRepository dictionaryItemRepository;

    public DictionaryController(IConfiguration configuration, IDictionaryRepository dictionaryRepository, IUnitOfWork1 unitOfWork1, IMapper mapper, IDictionaryItemRepository dictionaryItemRepository)
    {
        this.configuration = configuration;
        this.dictionaryRepository = dictionaryRepository;
        this.unitOfWork1 = unitOfWork1;
        this.mapper = mapper;
        this.dictionaryItemRepository = dictionaryItemRepository;
    }

    /// <summary>
    /// 添加字典
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResult<Dictionary>> AddDictionary([FromBody] AddDictionaryDto dto)
    {
        var dictionary = mapper.Map<AddDictionaryDto, Dictionary>(dto);
        await CheckDictionary(dictionary, false);
        unitOfWork1.BeginTransaction();
        var dbDictionary = await dictionaryRepository.InsertAsync(dictionary);

        unitOfWork1.Commit();
        return ApiResult<Dictionary>.Ok(dbDictionary);
    }

    /// <summary>
    /// 添加字典项
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResult<DictionaryItem>> AddDictionaryItem([FromBody] AddDictionaryItemDto dto)
    {
        var dictionaryItem = mapper.Map<AddDictionaryItemDto, DictionaryItem>(dto);
        await CheckDictionaryItem(dictionaryItem, false);
        unitOfWork1.BeginTransaction();
        var dbDictionary = await dictionaryItemRepository.InsertAsync(dictionaryItem);

        unitOfWork1.Commit();
        return ApiResult<DictionaryItem>.Ok(dbDictionary);
    }

    private async Task<bool> CheckDictionary(Dictionary dictionary, bool isUpdate)
    {
        var query = QueryCondition.True<Dictionary>();
        query = query.And(it => it.Code == dictionary.Code );
        if (isUpdate)
        {
            query = query.And(it => it.Id != dictionary.Id);
        }
     
        var dbModel = await dictionaryRepository.FirstOrDefaultAsync(query);

        if (dbModel != null)
        {
            throw new Exception("编码不允许重复");
        }
        
        return true;
    }
    [HttpPost]
    public async Task<ApiResult<Dictionary>> UpdateDictionary([FromBody] Dictionary dictionary)
    {
        unitOfWork1.BeginTransaction();
        await CheckDictionary(dictionary, true);
        var dbDictionary = await dictionaryRepository.GetAsync(dictionary.Id);
        if (dbDictionary == null)
        {
            throw new Exception("要修改的字典不存在");
        }

        await dictionaryRepository.UpdateAsync(dictionary);

        unitOfWork1.Commit();
        return ApiResult<Dictionary>.Ok(dbDictionary);
    }

    [HttpPost]
    public async Task<ApiResult<DictionaryItem>> UpdateDictionaryItem([FromBody] DictionaryItem dictionaryItem)
    {
        unitOfWork1.BeginTransaction();
        var dbDictionaryItem = await dictionaryItemRepository.GetAsync(dictionaryItem.Id);
        if (dbDictionaryItem == null)
        {
            throw new Exception("要修改的字典项目不存在");
        }

        await CheckDictionaryItem(dictionaryItem, true);

        await dictionaryItemRepository.UpdateAsync(dictionaryItem);

        unitOfWork1.Commit();
        return ApiResult<DictionaryItem>.Ok(dbDictionaryItem);
    }

    private async Task<bool> CheckDictionaryItem(DictionaryItem dictionaryItem, bool isUpdate)
    {
        var query = QueryCondition.True<DictionaryItem>();
        query = query.And(it => it.Name == dictionaryItem.Name&&it.DictionaryId==dictionaryItem.DictionaryId);
        if (isUpdate)
        {
            query = query.And(it => it.Id != dictionaryItem.Id);
        }
        var dbDictionaryItem = await dictionaryItemRepository.FirstOrDefaultAsync(query);
        if (dbDictionaryItem != null)
        {
            throw new Exception("相同名称的字典项已存在");
        }

        return true;
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteDictionarys([FromBody] DeleteMenusDto deleteMenusDto)
    {
        if (deleteMenusDto.Ids == null || deleteMenusDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        foreach (var id in deleteMenusDto.Ids)
        {
            await DeleteDictionarysByRecursion(id);
        }
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    [HttpPost]
    public async Task<ApiResult<bool>> DeleteDictionaryItems([FromBody] DeleteMenusDto deleteMenusDto)
    {
        if (deleteMenusDto.Ids == null || deleteMenusDto.Ids.Count == 0)
        {
            throw new Exception("要删除的id列表不能为空");
        }
        unitOfWork1.BeginTransaction();

        foreach (var id in deleteMenusDto.Ids)
        {
            await dictionaryItemRepository.DeleteAsync(it => it.Id == id);
        }
        unitOfWork1.Commit();
        return ApiResult<bool>.Ok(true);
    }

    private async Task<bool> DeleteDictionarysByRecursion(int dictionaryId)
    {
        await dictionaryRepository.DeleteAsync(it => it.Id == dictionaryId);
        await dictionaryItemRepository.DeleteAsync(x => x.DictionaryId == dictionaryId);
        var childrenMenus = await dictionaryRepository.Where(it => it.ParentId == dictionaryId).ToListAsync();
        if (childrenMenus.Count == 0)
        {
            return true;
        }

        foreach (var childrenDictionary in childrenMenus)
        {
            await DeleteDictionarysByRecursion(childrenDictionary.Id);
        }

        return true;
    }

    [HttpGet]
    public async Task<ApiResult<List<Dictionary>>> List()
    {
        var dictionarys = await dictionaryRepository.ToListAsync();

        var result = AddDictionarysTrees(dictionarys, null);
        return ApiResult<List<Dictionary>>.Ok(result);
    }

    [HttpPost]
    public async Task<ApiResult<Page<DictionaryItem>>> ListItem([FromBody] PageQueryDictionaryItemDto dto)
    {
        var dictionarys = await dictionaryItemRepository.Where(it => it.DictionaryId == dto.DictionaryId).OrderBy(x => x.Index).ToPageAsync(dto);

        return ApiResult<Page<DictionaryItem>>.Ok(dictionarys);
    }

    private List<Dictionary> AddDictionarysTrees(List<Dictionary> dictionarys, int? parentId)
    {
        var list = dictionarys.Where(it => it.ParentId == parentId).ToList();
        foreach (var dictionary in list)
        {
            var childrens = AddDictionarysTrees(dictionarys, dictionary.Id);
            if (dictionary.Children == null)
            {
                dictionary.Children = new List<Dictionary>();
            }
            dictionary.Children.AddRange(childrens);
        }

        return list;
    }

    [HttpGet]
    public async Task<ApiResult<List<DictionaryItem>>> GetDictionaryItems([FromQuery] string code)
    {
        var items = await dictionaryItemRepository
            .InnerJoin(new Dictionary(),x=>x.T1.DictionaryId==x.T2.Id)
            .Where(x=>x.T2.Code==code)
            .OrderBy(x => x.T1.Index)
            .Select(x=>x.T1)
            .ToListAsync();

        return ApiResult<List<DictionaryItem>>.Ok(items);
    }
}