using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Menu;

public class MenuMeta : BaseEntity
{
    [Description("菜单id")]
    public int MenuId { get; set; }
    /// <summary>
    /// 显示名称。展示在菜单，标签和面包屑等中
    /// </summary>
    [Description("显示名称。展示在菜单，标签和面包屑等中")]
    public string Title { get; set; }
    /// <summary>
    /// 类型：菜单，Iframe，外链，按钮
    /// </summary>
    [Description("类型：菜单，Iframe，外链，按钮")]
    public string Type { get; set; }
    /// <summary>
    /// 显示图标，建立2级菜单都设置图标，否则菜单折叠都将显示空白
    /// </summary>
    [Description("显示图标，建立2级菜单都设置图标，否则菜单折叠都将显示空白")]
    public string? Icon { get; set; }
    /// <summary>
    /// 是否固定，类似首页控制台在标签中是没有关闭按钮的
    /// </summary>
    [Description("是否固定，类似首页控制台在标签中是没有关闭按钮的")]
    public bool? Affix { get; set; }
    /// <summary>
    /// 是否隐藏菜单，大部分用在无需显示在左侧菜单中的页面，比如详情页
    /// </summary>
    [Description("是否隐藏菜单，大部分用在无需显示在左侧菜单中的页面，比如详情页")]
    public bool? Hidden { get; set; }
    /// <summary>
    /// 静态路由时，所能访问路由的角色，例如：role: ["SA"]
    /// </summary>
    [Description("静态路由时，所能访问路由的角色，例如：role: [\"SA\"]")]
    [NotMapped]
    public List<string> Role { get; set; }
    /// <summary>
    /// 左侧菜单的路由地址活动状态，比如打开详情页需要列表页的菜单活动状态
    /// </summary>
    [Description("左侧菜单的路由地址活动状态，比如打开详情页需要列表页的菜单活动状态")]
    public string? MenuActive { get; set; }
    /// <summary>
    /// 是否隐藏面包屑
    /// </summary>
    [Description("是否隐藏面包屑")]
    public bool? HiddenBreadcrumb { get; set; }
    /// <summary>
    /// 颜色值
    /// </summary>
    [Description("颜色值")]
    public string Color { get; set; }
    /// <summary>
    /// 是否整页打开路由（脱离框架系），例如：fullpage: true
    /// </summary>
    [Description("是否整页打开路由（脱离框架系），例如：fullpage: true")]
    public bool? Fullpage { get; set; }


}