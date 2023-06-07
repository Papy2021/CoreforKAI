using Microsoft.AspNetCore.Authorization;

namespace Core.Security
{
    public class SuperAdminHandler:AuthorizationHandler<ManageAdminRolesClaimsRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ManageAdminRolesClaimsRequirements requirement)
        {
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
