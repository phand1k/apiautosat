using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Services.Implementations;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AvtoMigBussines.Detailing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Директор")]
    [CheckSubscription]
    public class DetailingDirectorController : Controller
    {
        private readonly IDetailingOrderService detailingOrderService;
        private readonly IUserService _userService;
        private readonly UserManager<AspNetUser> userManager;
        public DetailingDirectorController(UserManager<AspNetUser> userManager, IDetailingOrderService detailingOrderService, IUserService userService)
        {
            this.detailingOrderService = detailingOrderService;
            _userService = userService;
            this.userManager = userManager;
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
        [HttpPost("CreateDetailingOrder")]
        public async Task<IActionResult> CreateDetailingOrder([FromBody] DetailingOrder detailingOrder)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await detailingOrderService.CreateDetailingOrderAsync(detailingOrder, user.Id);


                return Ok(detailingOrder);
            }
            catch (CustomException.WashOrderExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while creating the wash order.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." + ex.Message });
            }
        }
    }
}
