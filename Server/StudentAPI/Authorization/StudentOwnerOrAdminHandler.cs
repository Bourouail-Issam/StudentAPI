using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// This authorization handler enforces the ownership rule for student resources.
// It checks whether the current user is either:
// - An Owner (full access), OR
// - The owner of the student record being requested

namespace StudentAPI.Authorization
{
    public class StudentOwnerOrAdminHandler
        : AuthorizationHandler<StudentOwnerOrAdminRequirement,int>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            StudentOwnerOrAdminRequirement requirement,
            int ownerUserId)
        {
            // Admin override
            if (context.User.IsInRole("Owner"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Ownership check
            var sub = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (int.TryParse(sub, out int authenticatedStudentId) &&
                authenticatedStudentId == ownerUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
