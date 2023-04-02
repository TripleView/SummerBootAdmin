using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace SummerBootAdmin.Dto.Dictionary;

public class AddDictionaryDto
{
    [Description("父字典id")]
    public int? ParentId { get; set; }
    [Description("字典名称")]
    public string Name { get; set; }

    [Description("编码")]
    public string Code { get; set; }


}