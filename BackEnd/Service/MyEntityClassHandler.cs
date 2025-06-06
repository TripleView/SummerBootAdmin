using SummerBoot.Core;
using SummerBoot.Repository;
using System.Security.Claims;

namespace SummerBootAdmin.Service;

public class MyEntityClassHandler : IEntityClassHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public MyEntityClassHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public void ProcessingEntity(IBaseEntity entity, bool isUpdate = false)
    {
        if (entity is BaseEntity baseEntity)
        {
            var user = httpContextAccessor.HttpContext?.User;
            var account = user?.FindFirst("account")?.Value;
            baseEntity.LastUpdateBy = account;
            baseEntity.LastUpdateOn = DateTime.UtcNow;
            if (isUpdate == false)
            {
                baseEntity.CreateBy = account;
                baseEntity.CreateOn = DateTime.UtcNow;
            }
        }
        
    }

    public async Task ProcessingEntityAsync(IBaseEntity entity, bool isUpdate = false)
    {
        if (entity is BaseEntity baseEntity)
        {
            var user = httpContextAccessor.HttpContext?.User;
            var account = user?.FindFirst("account")?.Value;
            baseEntity.LastUpdateBy = account;
            baseEntity.LastUpdateOn = DateTime.UtcNow;
            if (isUpdate == false)
            {
                baseEntity.CreateBy = account;
                baseEntity.CreateOn = DateTime.UtcNow;
            }
        }
    }
}