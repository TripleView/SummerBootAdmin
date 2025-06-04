using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Menu;

public class Menu : BaseEntity
{
    [Description("父菜单id")]
    public int? ParentId { get; set; }
    [Description("菜单名称")]
    public string Name { get; set; }
    [Description("菜单路径")]
    public string Path { get; set; }
    [Description("子菜单列表")]
    [NotMapped]
    public List<Menu> Children { get; set; }
    [Description("菜单元数据")]
    [NotMapped]
    public MenuMeta Meta { get; set; }
    [Description("菜单组件")]
    public string Component { get; set; }
    [Description("重定向地址。")]
    public string Redirect { get; set; }
    [NotMapped]
    public List<MenuApiMapping> ApiList { get; set; }
}