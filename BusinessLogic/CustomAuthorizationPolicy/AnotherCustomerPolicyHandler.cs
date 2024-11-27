using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CustomAuthorizationPolicy
{
    public class AnotherCustomerPolicyHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            var isnull = context.Resource as HttpContext;
            if(isnull == null) 
                return Task.CompletedTask; 

            if (context.User.IsInRole("Visitor"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
