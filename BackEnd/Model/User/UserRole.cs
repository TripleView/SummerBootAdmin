using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.User;
[Description("用户角色表")]
public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}