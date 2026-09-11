using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using SummerBoot.Repository;
namespace SummerBootAdmin.Model.CodeGenerator;
/// <summary>
/// 代码生成设置
/// </summary>
[Description("代码生成设置")]
public class CodeGeneratorSetting : BaseEntity
{
    /// <summary>
    /// 主目录
    /// </summary>
    [Description("主目录")]
    public string Workspace { get; set; }
    /// <summary>
    /// 实体类目录
    /// </summary>
    [Description("实体类目录")]
    public string ModelDirectory { get; set; }
    /// <summary>
    /// 仓储目录
    /// </summary>
    [Description("仓储目录")]
    public string RepositoryDirectory { get; set; }
    /// <summary>
    /// 服务目录
    /// </summary>
    [Description("服务目录")]
    public string ServiceDirectory { get; set; }
    /// <summary>
    /// 控制器目录
    /// </summary>
    [Description("控制器目录")]
    public string ControllerDirectory { get; set; }
}