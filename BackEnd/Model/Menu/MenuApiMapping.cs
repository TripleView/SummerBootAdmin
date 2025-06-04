using System.ComponentModel;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Menu;

public class MenuApiMapping : BaseEntity
{
    [Description("菜单id")]
    public int MenuId { get; set; }
    [Description("Api Url")]
    public string ApiUrl { get; set; }
}