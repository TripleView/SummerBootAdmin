using System.ComponentModel;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Role;
[Description("角色")]
public class Role:BaseEntity
{
    [Description("角色名称")]
    public string Name { get; set; }
    [Description("备注")]
    public string Remark { get; set; }
}