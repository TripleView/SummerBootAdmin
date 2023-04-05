using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Department;

[Description("部门")]
public class Department : BaseEntity
{
    [Description("父部门id")]
    public int? ParentId { get; set; }
    [Description("部门名称")]
    public string Name { get; set; }

    [Description("子部门列表")]
    [NotMapped]
    public List<Department> Children { get; set; }

    [Description("顺序")]
    public int Sort { get; set; }
    [Description("备注")]
    public string Remark { get; set; }

}