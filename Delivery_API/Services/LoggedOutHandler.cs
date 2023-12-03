using Microsoft.AspNetCore.Authorization;

namespace Delivery_API.Services
{
    public class LoggedOutRequirement : IAuthorizationRequirement { }
    public class LoggedOutHandler : AuthorizationHandler<LoggedOutRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoggedOutRequirement requirement)
        {
            // 检查用户是否已注销
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
