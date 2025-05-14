using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace SummerBootAdmin.Dto.Dictionary;

public class AddDictionaryItemDto
{
    [Description("字典id")]
    public int DictionaryId { get; set; }
    [Description("字典项名称")]
    public string Name { get; set; }
    [Description("字典项值")]
    public string Value { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    public int Index { get; set; }

}