using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AvtoMigBussines.CarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Мастер")]
    public class MasterController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IServiceService _serviceService;
        private readonly IWashServiceService _washService;
        public MasterController(IUserService userService, UserManager<AspNetUser> userManager, IServiceService serviceService, IWashServiceService washServiceService)
        {
            _userService = userService;
            this.userManager = userManager;
            _serviceService = serviceService;
            _washService = washServiceService;
        }
        private async Task<AspNetUser> GetCurrentUserAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var aspNetUser = await _userService.GetUserByPhoneNumberAsync(userName);
            if (aspNetUser == null)
            {
                return null;
            }

            var user = await userManager.FindByIdAsync(aspNetUser.Id);
            return user;
        }
        [HttpGet("GetAllMyServices")]
        public async Task<IActionResult> GetAllMyServices()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var MyWashServices = await _washService.GetAllMyIsNotCompletedWashServices(user.Id);
            return Ok(MyWashServices);
        }
        [HttpGet("GetAllMyCompletedServices")]
        public async Task<IActionResult> GetAllMyCompletedServices()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var MyWashServices = await _washService.GetAllMyWashServices(user.Id);
            return Ok(MyWashServices);
        }
        [HttpPatch("CompleteWashService")]
        public async Task<IActionResult> CompleteWashService([Required] int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return BadRequest();
            }
            var washService = await _washService.GetByIdAsync(id);
            if (washService == null)
            {
                return NotFound(new { Message = "Wash service not found." });
            }
            await _washService.CompleteUpdateAsync(washService);
            return Ok(washService);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSalary()
        {
            return Ok();
        }
    }
}
