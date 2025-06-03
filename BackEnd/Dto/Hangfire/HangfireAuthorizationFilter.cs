using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace SummerBootAdmin.Dto.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IHostEnvironment env;

    public HangfireAuthorizationFilter(IHostEnvironment env)
    {
        this.env = env;
    }
    public bool Authorize([NotNull] DashboardContext context)
    {
        if (!env.IsProduction())
        {
            return true;
        }

        var httpContext = context.GetHttpContext();

        var header = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrWhiteSpace(header))
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        var authValues = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);

        if (!"Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        var parameter = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
        var parts = parameter.Split(':');

        if (parts.Length < 2)
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        var username = parts[0];
        var password = parts[1];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        if (username == "admin" && password == "123")
        {
            return true;
        }

        SetChallengeResponse(httpContext);
        return false;
    }

    private void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
        httpContext.Response.WriteAsync("Authentication is required.");
    }
}
