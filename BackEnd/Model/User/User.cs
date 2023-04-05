using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.User;
[Description("用户")]
public class User:BaseEntity
{
    [Description("账号")]
    public string Account { get; set; }
    [Description("姓名")]
    public string Name { get; set; }
    [Description("角色id")]
    public int RoleId { get; set; }
    [Description("角色名称")]
    [NotMapped]
    public string RoleName { get; set; }
    [Description("部门id")]
    public int DepartmentId { get; set; }
    [Description("密码")]
    public string Password { get; set; }
    [Description("头像")]
    public string Avatar { get; set; }
}