namespace SummerBootAdmin.Dto.Menu;

public class MenuOutPutDto
{
    public List<string> DashboardGrid { get; set; }
    public List<MenuItem> Menu { get; set; }
    public List<string> Permissions { get; set; }
}

public class MenuItem
{
    public string Name { get; set; }
    public string Path { get; set; }
    public List<MenuItem> Children { get; set; }
    public MenuMeta Meta { get; set; }
    public string Component { get; set; }

}

public class MenuMeta
{
    /// <summary>
    /// 显示名称。展示在菜单，标签和面包屑等中
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 类型：菜单，Iframe，外链，按钮
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// 显示图标，建立2级菜单都设置图标，否则菜单折叠都将显示空白
    /// </summary>
    public string? Icon { get; set; }
    /// <summary>
    /// 是否固定，类似首页控制台在标签中是没有关闭按钮的
    /// </summary>
    public bool? Affix { get; set; }
    /// <summary>
    /// 是否隐藏菜单，大部分用在无需显示在左侧菜单中的页面，比如详情页
    /// </summary>
    public bool? Hidden { get; set; }
    /// <summary>
    /// 静态路由时，所能访问路由的角色，例如：role: ["SA"]
    /// </summary>
    public List<string> Role { get; set; }
    /// <summary>
    /// 左侧菜单的路由地址活动状态，比如打开详情页需要列表页的菜单活动状态
    /// </summary>
    public string? Active { get; set; }
    /// <summary>
    /// 是否隐藏面包屑
    /// </summary>
    public bool? HiddenBreadcrumb { get; set; }
}