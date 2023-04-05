using System.Reflection.Metadata.Ecma335;
using SummerBoot.Repository;

namespace SummerBootAdmin.Dto.Dictionary;

public class PageQueryDictionaryItemDto : IPageable
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int DictionaryId { get; set; }
}