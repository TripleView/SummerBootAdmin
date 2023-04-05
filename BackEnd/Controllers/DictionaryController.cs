using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.ExpressionParser.Parser;
using SummerBootAdmin.Dto.Dictionary;
using SummerBootAdmin.Dto.Menu;
using SummerBootAdmin.Model;
using SummerBootAdmin.Model.Dictionary;
using SummerBootAdmin.Repository;
using SummerBootAdmin.Repository.Dictionary;

namespace SummerBootAdmin;

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

    [HttpPost]
    public async Task<ApiResult<Dictionary>> AddDictionary([FromBody] AddDictionaryDto dto)
    {
        var dictionary = mapper.Map<AddDictionaryDto, Dictionary>(dto);
        unitOfWork1.BeginTransaction();
        var dbDictionary = await dictionaryRepository.InsertAsync(dictionary);

        unitOfWork1.Commit();
        return ApiResult<Dictionary>.Ok(dbDictionary);
    }

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


    [HttpPost]
    public async Task<ApiResult<Dictionary>> UpdateDictionary([FromBody] Dictionary dictionary)
    {
        unitOfWork1.BeginTransaction();
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
        query = query.And(it => it.Name == dictionaryItem.Name);
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
        var dictionarys = await dictionaryItemRepository.Where(it => it.DictionaryId == dto.DictionaryId).ToPageAsync(dto);

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


}