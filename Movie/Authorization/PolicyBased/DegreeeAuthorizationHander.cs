using Microsoft.AspNetCore.Authorization;

namespace Movie.Authorization.PolicyBased
{
    public class DegreeeAuthorizationHander : AuthorizationHandler<DegreeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DegreeRequirement requirement)
        {
             bool isPass=int.TryParse(context.User.FindFirst("Degree").Value,out int result);
            if (isPass) {
                int degree = result;
                if (degree >= requirement.Degree)
                    context.Succeed(requirement);
            }
         
            return Task.CompletedTask;
        }
    }
}
