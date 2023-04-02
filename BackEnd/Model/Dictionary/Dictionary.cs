using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Dictionary;

public class Dictionary : BaseEntity
{
    [Description("父字典id")]
    public int? ParentId { get; set; }
    [Description("字典名称")]
    public string Name { get; set; }
   
    [Description("子菜单列表")]
    [NotMapped]
    public List<Dictionary> Children { get; set; }

    [Description("编码")]
    public string Code { get; set; }


}