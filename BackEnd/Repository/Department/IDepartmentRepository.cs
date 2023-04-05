using SummerBoot.Repository;
using SummerBootAdmin.Model.Department;

namespace SummerBootAdmin.Repository.Department;

[AutoRepository1]
public interface IDepartmentRepository : IBaseRepository<Model.Department.Department>
{
    
}