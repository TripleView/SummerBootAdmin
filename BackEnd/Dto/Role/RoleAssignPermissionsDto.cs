namespace SummerBootAdmin.Dto.Role;

public class RoleAssignPermissionsDto
{
    /// <summary>
    /// 角色id
    /// </summary>
    public List<int> RoleIds { get; set; }
    /// <summary>
    /// 菜单id
    /// </summary>
    public List<int> MenuIds { get; set; }
}