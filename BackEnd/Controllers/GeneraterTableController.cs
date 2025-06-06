using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SummerBoot.Repository.Generator;
using SummerBootAdmin.Model;
using SummerBootAdmin.Model.Department;
using SummerBootAdmin.Model.Dictionary;
using SummerBootAdmin.Model.Menu;
using SummerBootAdmin.Model.Role;
using SummerBootAdmin.Model.User;

namespace SummerBootAdmin.Controllers;
[Authorize(Policy = "urlPolicy")]
[ApiController]
[Route("api/[controller]/[action]")]
public class GeneraterTableController : Controller
{
    private readonly IDbGenerator1 dbGenerator1;

    public GeneraterTableController(IDbGenerator1 dbGenerator1)
    {
        this.dbGenerator1 = dbGenerator1;
    }
    [HttpGet]
    public IActionResult Index()
    {
        var sqlResults= dbGenerator1.GenerateSql(new List<Type>()
        {
            typeof(Department), typeof(Dictionary), typeof(DictionaryItem),
            typeof(Role), typeof(RoleAssignMenu),typeof(UserRole),
            typeof(User), typeof(MenuMeta), typeof(Menu),typeof(MenuApiMapping)
        });
        foreach (var sqlResult in sqlResults)
        {
            dbGenerator1.ExecuteGenerateSql(sqlResult);
        }
        return Content("ok");
    }
}