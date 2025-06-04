using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SummerBoot.Cache;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBootAdmin.Model.Menu;
using SummerBootAdmin.Repository.Menu;
using SummerBootAdmin.Repository.Role;

namespace SummerBootAdmin.Dto.Login;

public class SummerBootAuthorizationHandler : AuthorizationHandler<SummerBootRequirement>
{
    private readonly IMenuApiMappingRepository menuApiMappingRepository;
    private readonly IRoleAssignMenuRepository roleAssignMenuRepository;
    private readonly ICache cache;
    private readonly IHttpContextAccessor httpContextAccessor;

    public SummerBootAuthorizationHandler(IMenuApiMappingRepository menuApiMappingRepository, IRoleAssignMenuRepository roleAssignMenuRepository, ICache cache, IHttpContextAccessor httpContextAccessor)
    {
        this.menuApiMappingRepository = menuApiMappingRepository;
        this.roleAssignMenuRepository = roleAssignMenuRepository;
        this.cache = cache;
        this.httpContextAccessor = httpContextAccessor;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SummerBootRequirement requirement)
    {
        var roles = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        if (roles.Contains("admin"))
        {
            context.Succeed(requirement);
        }
        else
        {
            var roleIds = context.User.Claims.Where(x => x.Type == "roleId").Select(x => int.Parse(x.Value)).ToList();
            var url = httpContextAccessor.HttpContext?.Request.Path.Value ?? "";
            if (url.IsNullOrWhiteSpace())
            {
                context.Fail();
                return;
            }

            url = url.TrimStart('/').ToLower();
            if (url == "api/menu/getmenus")
            {
                context.Succeed(requirement);
                return;
            }
            var cacheKey = LoginConst.RoleApisKey;
            var allRoleApiCache = await cache.GetValueAsync<Dictionary<int, HashSet<string>>>(cacheKey);
            var allRoleApis = new Dictionary<int, HashSet<string>>();
            if (allRoleApiCache.HasValue)
            {
                allRoleApis = allRoleApiCache.Data;
            }
            else
            {
                var dbRoleApiDtos = await roleAssignMenuRepository.InnerJoin(new Model.Role.Role(), x => x.T1.RoleId == x.T2.Id)
                    .InnerJoin(new MenuApiMapping(), x => x.T1.MenuId == x.T3.MenuId)
                    .Where(x => roleIds.Contains(x.T2.Id))
                    .Select(x => new RoleApiDto() { RoleId = x.T1.RoleId, ApiUrl = x.T3.ApiUrl })
                    .ToListAsync();

                allRoleApis = dbRoleApiDtos.GroupBy(x => x.RoleId).ToDictionary(x => x.Key, x => x.ToList().Select(z => z.ApiUrl.ToLower()).ToHashSet());
                await cache.SetValueWithAbsoluteAsync(cacheKey, allRoleApis, TimeSpan.FromDays(1));
            }
            foreach (var roleId in roleIds)
            {
                if (allRoleApis.TryGetValue(roleId, out var apis) && apis.Contains(url))
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            context.Fail();

        }

    }
}