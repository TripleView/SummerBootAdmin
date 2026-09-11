using SummerBoot.Repository;
using System.ComponentModel;

namespace SummerBootAdmin.Model.CodeGenerator;
[Description("字段类型")]
public class DatabaseEntityFieldType : BaseEntity
{
    [Description("字段类型名称")]
    public string Name { get; set; }

    [Description("字段类型")]
    public string Value { get; set; }

    [Description("描述")]
    public string Description { get; set; }
    [Description("排序")]
    public int OrderIndex { get; set; }
}