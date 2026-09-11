using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SummerBoot.Repository;

namespace SummerBootAdmin.Model.CodeGenerator;

[Description("数据库实体类")]
public class DatabaseEntityClass : BaseEntity
{
    [Description("实体类名")]
    public string Name { get; set; }
    [Description("描述")]
    public string Description { get; set; }
    [NotMapped]
    public List<DatabaseEntityField> Fields { get; set; }
}