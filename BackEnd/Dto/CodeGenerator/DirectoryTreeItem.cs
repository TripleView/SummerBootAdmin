namespace SummerBootAdmin.Dto.CodeGenerator;

public class DirectoryTreeItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public List<DirectoryTreeItem> Children { get; set; }
}