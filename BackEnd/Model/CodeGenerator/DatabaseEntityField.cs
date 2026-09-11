using System.ComponentModel;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.CodeGenerator;

[Description("数据库实体类的字段名")]
public class DatabaseEntityField : BaseEntity
{
    [Description("字段名")]
    public string Name { get; set; }
    [Description("描述")]
    public string Description { get; set; }

    [Description("是否可空")]
    public bool IsEmpty { get; set; }
    [Description("数据库实体类Id")]
    public int ClassId { get; set; }
    [Description("字段类型id")]
    public int FieldTypeId { get; set; }
    [Description("排序")]
    public int OrderIndex { get; set; }
}