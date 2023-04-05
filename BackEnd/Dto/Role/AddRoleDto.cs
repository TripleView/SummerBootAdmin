using System.ComponentModel;

namespace SummerBootAdmin.Dto.Role;

public class AddRoleDto
{
    [Description("角色名称")]
    public string Name { get; set; }
    [Description("备注")]
    public string Remark { get; set; }
}