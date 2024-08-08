using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<AspNetUser> userManager;
        private readonly INotificationCenterService notificationCenterService;
        public ProfileController(IUserService userService, UserManager<AspNetUser> userManager, INotificationCenterService notificationCenterService)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.notificationCenterService = notificationCenterService;
        }

        private async Task<AspNetUser> GetCurrentUserAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var aspNetUser = await userManager.FindByEmailAsync(userName);
            if (aspNetUser == null)
            {
                return null;
            }

            var user = await userManager.FindByIdAsync(aspNetUser.Id);
            return user;
        }
        [HttpPatch("EditProfile")]
        public async Task<IActionResult> EditProfile(AspNetUser aspNetUser)
        {
            var user = await GetCurrentUserAsync();
            await userService.UpdateUserAsync(aspNetUser);
            return Ok("User profile success updated");
        }

        [HttpGet("Notifications")]
        public async Task<IActionResult> Notifications()
        {
            var notificationData = await notificationCenterService.GetAllNotificationsAsync();
            return Ok(notificationData);
        }
        [Route("GetProfileInfo")]
        [HttpGet]
        public async Task<IActionResult> GetProfileInfo()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var aspNetUser = await userManager.FindByEmailAsync(userName);
            var user = await userManager.FindByIdAsync(aspNetUser.Id);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

    }
}
