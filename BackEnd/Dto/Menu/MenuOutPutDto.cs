using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SummerBootAdmin.Dto.Menu;

public class MenuOutPutDto
{
    public List<string> DashboardGrid { get; set; }
    public List<Model.Menu> Menu { get; set; }
    public List<string> Permissions { get; set; }
}

