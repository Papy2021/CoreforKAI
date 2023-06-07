using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Core.Security
{
    public class CanEditOnlyOtherAdminsRolesClaims:AuthorizationHandler<ManageAdminRolesClaimsRequirements>
    {

#pragma warning disable CS8602 // Possible null reference return.
#pragma warning disable CS8600 // Possible null reference return.


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ManageAdminRolesClaimsRequirements requirement)
        {
            //var authFilterContext = context.Resource as AuthorizationFilterContext;
            //if (authFilterContext==null) { 
            //    return Task.CompletedTask;
            //}

            if (context.Resource is not AuthorizationFilterContext authFilterContext)
            {
                return Task.CompletedTask;
            }

            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];


            if (context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Edit Role"
                                               && claim.Value == "True")
                                               && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
               
            }
            return Task.CompletedTask;
        }
    }
}
