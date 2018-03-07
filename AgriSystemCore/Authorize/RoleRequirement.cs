using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgriSystemCore.Authorize
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public List<string> Roles { get; private set; }

        public RoleRequirement(List<string> roles)
        {
            //--在這邊把db裡面需要的 role 填進來
            this.Roles = roles;
        }
    }

    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                return Task.CompletedTask;
            }

            if (requirement.Roles.Any(x => context.User.IsInRole(x)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
