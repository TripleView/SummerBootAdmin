using SummerBoot.Repository;
using System.ComponentModel;

namespace SummerBootAdmin.Model.Role;
[Description("角色分配菜单")]
public class RoleAssignMenu : BaseEntity
{
    [Description("菜单id")]
    public int MenuId { get; set; }
    [Description("角色id")]
    public int RoleId { get; set; }
}