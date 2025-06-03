using SummerBoot.Repository;

namespace SummerBootAdmin.Dto.User;

public class PageQueryUserDto : Pageable
{
    public int? DepartmentId { get; set; }

    public string Name { get; set; }
}