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

        //if you write context.Fail() so you gurantee that if the other custom policy return context.Succeed(requirement)
        //it won't give the access at all so if you want to take the access based on another custom policies just return Task.CompletedTask
        //if you want to prevent code from checking other custom policy after setting this policy context.Fail()
        //just in the program set this line     options.InvokeHandlersAfterFailure = false;

        //else
        //    context.Fail();
        return Task.CompletedTask;
    }
}
