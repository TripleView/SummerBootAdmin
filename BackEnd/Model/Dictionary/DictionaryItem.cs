using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.Dictionary;

[Description("字典项")]
public class DictionaryItem : BaseEntity
{
    [Description("字典id")]
    public int DictionaryId { get; set; }
    [Description("字典项名称")]
    public string Name { get; set; }
    [Description("字典项值")]
    public string Value { get; set; }
    [Description("排序")]
    [Column("orderIndex")]
    public int Index { get; set; }
}