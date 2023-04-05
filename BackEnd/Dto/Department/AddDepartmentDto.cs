using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SummerBootAdmin.Dto.Department;

public class AddDepartmentDto
{
    [Description("父部门id")]
    public int? ParentId { get; set; }
    [Description("部门名称")]
    public string Name { get; set; }

    [Description("顺序")]
    public int Sort { get; set; }
    [Description("备注")]
    public string Remark { get; set; }


}