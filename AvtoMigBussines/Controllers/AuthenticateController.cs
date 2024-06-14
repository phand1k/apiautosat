using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Authenticate.Models;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static AvtoMigBussines.Exceptions.CustomException;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : Controller
    {
        private readonly IUserService userService;
        public AuthenticateController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
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
