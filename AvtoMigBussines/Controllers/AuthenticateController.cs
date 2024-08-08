using AvtoMigBussines.Attributes;
using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Authenticate.Models;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static AvtoMigBussines.Exceptions.CustomException;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<AspNetUser> userManager;
        public AuthenticateController(IUserService userService, UserManager<AspNetUser> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }
        [Authorize]
        [HttpGet("CheckUserFullName")]
        public async Task<IActionResult> CheckUserFullName()
        {
            var user = await GetCurrentUserAsync();
            if (user.FirstName == null || user.LastName == null)
            {
                return StatusCode(204);
            }
            return Ok();
        }
        [Authorize]
        [HttpPatch("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string? id)
        {
            await userService.DeleteUserAsync(id);
            return Ok("Succes for delete user: " + id);
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
        [HttpPost("InviteUser")]
        public async Task<IActionResult> InviteUser(string? phoneNumber)
        {

            return Ok();
        }
        [HttpGet]
        [Authorize]
        [Route("GetRole")]
        public async Task<IActionResult> GetRole()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var role = await userManager.GetRolesAsync(user);
            return Ok(role);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.PhoneNumber);
                if (user == null || user.IsDeleted == true)
                {
                    return Unauthorized(new { Message = "Invalid username or password, or the account has been deleted." });
                }

                if (!await userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Unauthorized(new { Message = "Invalid username or password." });
                }

                var token = await userService.LoginUserAsync(model);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([Required] [FromBody] RegisterModel model)
        {
            try
            {
                await userService.RegisterUserAsync(model);
                return Ok(new { Message = "Registration successful" });
            }
            catch (UserAlreadyExistsException ex)
            {
                return StatusCode(401, new { Message = ex.Message });
            }
            catch (OrganizationNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request" });
            }
        }
    }
}
