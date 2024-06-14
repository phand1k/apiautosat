using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private readonly IOrganizationService organizationService;
        private readonly ISubscriptionService subscriptionService;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IUserService userService;
        public OrganizationController(IOrganizationService organizationService, ISubscriptionService subscriptionService, UserManager<AspNetUser> userManager, IUserService userService)
        {
            this.organizationService = organizationService;
            this.subscriptionService = subscriptionService;
            this.userManager = userManager;
            this.userService = userService;
        }
        [Route("CreateOrganization")]
        [HttpPost]
        public async Task<IActionResult> CreateOrganization([Required][FromBody] Organization organization)
        {
            try
            {
                await organizationService.CreateOrganizationAsync(organization);

                await subscriptionService.CreateSubscriptionAsync(organization.Id);
                return Ok(organization);
            }
            catch (CustomException.OrganizationExistsException ex)
            {
                return StatusCode(400, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }
        [Route("GetSubscriptionOrganization")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionOrganization()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var aspNetUser = await userService.GetUserByPhoneNumberAsync(userName);
            var user = await userManager.FindByIdAsync(aspNetUser.Id);
            if (user == null)
            {
                return Unauthorized();
            }
            var organization = await subscriptionService.GetSubscriptionById(user.OrganizationId);

            return Ok(organization);
        }

        [Route("GetOrganization")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrganization()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var aspNetUser = await userService.GetUserByPhoneNumberAsync(userName);
            var user = await userManager.FindByIdAsync(aspNetUser.Id);
            if (user == null)
            {
                return Unauthorized();
            }
            var organization = await organizationService.GetOrganizationByIdAsync(user.OrganizationId);

            return Ok(organization);
        }

    }
}
