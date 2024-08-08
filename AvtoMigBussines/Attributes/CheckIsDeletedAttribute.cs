using AvtoMigBussines.Authenticate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AvtoMigBussines.Attributes
{
    public class CheckIsDeletedAttribute : TypeFilterAttribute
    {
        public CheckIsDeletedAttribute() : base(typeof(CheckIsDeletedFilter))
        {
        }

        private class CheckIsDeletedFilter : IAsyncActionFilter
        {
            private readonly UserManager<AspNetUser> _userManager;

            public CheckIsDeletedFilter(UserManager<AspNetUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var userId = GetUserIdFromClaims(context.HttpContext.User);
                    if (userId != null)
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null && user.IsDeleted == true)
                        {
                            context.Result = new ObjectResult(new
                            {
                                message = "Your account has been deleted.",
                                action = "Please contact support for more information."
                            })
                            {
                                StatusCode = StatusCodes.Status403Forbidden
                            };

                            return;
                        }
                    }
                }
                await next();
            }

            private string GetUserIdFromClaims(ClaimsPrincipal user)
            {
                return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }
    }
}
