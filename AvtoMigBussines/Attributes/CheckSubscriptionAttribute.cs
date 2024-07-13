using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class CheckSubscriptionAttribute : TypeFilterAttribute
{
    public CheckSubscriptionAttribute() : base(typeof(CheckSubscriptionFilter))
    {
    }

    private class CheckSubscriptionFilter : IAsyncActionFilter
    {
        private readonly INotificationService _notificationService;

        public CheckSubscriptionFilter(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var organizationId = GetOrganizationIdFromUser(context.HttpContext.User);
                if (organizationId != null)
                {
                    var isSubscriptionExpired = await _notificationService.CheckAndNotifySubscriptionExpiryAsync(organizationId.Value);
                    if (isSubscriptionExpired)
                    {
                        context.Result = new ObjectResult(new
                        {
                            message = "Subscription expired. Please renew your subscription.",
                            action = "Please visit your account page to renew your subscription."
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


        private int? GetOrganizationIdFromUser(ClaimsPrincipal user)
        {
            var organizationIdClaim = user.FindFirst("OrganizationId");
            if (organizationIdClaim != null && int.TryParse(organizationIdClaim.Value, out int organizationId))
            {
                return organizationId;
            }
            return null;
        }
    }
}
