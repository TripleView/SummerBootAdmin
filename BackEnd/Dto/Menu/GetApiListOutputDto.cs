namespace SummerBootAdmin.Dto.Menu;

public class GetApiListOutputDto
{
    /// <summary>
    /// url地址
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// 文档注释
    /// </summary>
    public string Summary { get; set; }
    /// <summary>
    /// 方法详情
    /// </summary>
    public string ActionName { get; set; }
}