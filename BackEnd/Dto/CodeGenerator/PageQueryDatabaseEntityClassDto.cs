using SummerBoot.Repository;
using System.ComponentModel;

namespace SummerBootAdmin.Dto.CodeGenerator;

public class PageQueryDatabaseEntityClassDto : IPageable
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public List<OrderByItem> OrderByItems { set; get; }

    public string Name { get; set; }


    public string Description { get; set; }
}