using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BusinessLogic.CustomAuthorizationPolicy;

public class CanEditOnlyAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ManageAdminRolesAndClaimsRequirement requirement)
    {
        var authFilterContext = context.Resource as HttpContext;
        if (authFilterContext == null)
            return Task.CompletedTask;
       
        string Id = authFilterContext!.Request.Query["userId"].ToString();

        if (context.User.HasClaim(x => x.Type == "Role" && x.Value == "edit") && context.User.IsInRole("Super Admin")
            && context.User.HasClaim(x => x.Type == "ID" && x.Value.ToString() != Id))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
