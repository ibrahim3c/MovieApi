using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movie.Data;
using Movie.Models;
using System.Security.Claims;

namespace Movie.Authorization.PermissionBased
{
    public class PermissionBasedAuthorizationFilter(AppDbContext appDbContext) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata.FirstOrDefault(a => a is CheckPermissionAttribute) as CheckPermissionAttribute;
            if (attribute != null)
            {
                string UserID = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value.ToString();
                if (!context.HttpContext.User.Identity.IsAuthenticated || UserID == null)
                {
                    context.Result = new ForbidResult()
;
                }
                else
                {
                    // get permissions of user from db
                    var HasThisPermissions = appDbContext.Set<UserPermission>().Any(x => x.UserId == UserID && (PermissionEnum)x.PermissionId == attribute.Permission);
                    if (!HasThisPermissions) context.Result = new ForbidResult();

                }
            }
        }
    }
}
