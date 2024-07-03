using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Директор")]
    public class UserRoleController : Controller
    {
        private ApplicationDbContext _context;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AspNetUser> userManager;
        private readonly IUserService _userService;
        public UserRoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AspNetUser> userManager, IUserService userService)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _userService = userService;
        }
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> Users()
        {
            var users = await userManager.Users.
                Where(x=>x.OrganizationId == GetCurrentUserAsync().Result.OrganizationId).ToListAsync();
            return Ok(users);
        }
        [HttpPatch("EditUserRole")]
        public async Task<IActionResult> EditUserRole([Required] string userId, [Required] string roleId)
        {
            // Получаем пользователя по userId
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Получаем роль по roleId
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found" });
            }

            // Получаем текущие роли пользователя
            var userRoles = await userManager.GetRolesAsync(user);

            // Удаляем все текущие роли пользователя
            var removeResult = await userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest(new { Message = "Failed to remove user roles", Errors = removeResult.Errors });
            }

            // Добавляем новую роль пользователю
            var addResult = await userManager.AddToRoleAsync(user, role.Name);
            if (!addResult.Succeeded)
            {
                return BadRequest(new { Message = "Failed to add user role", Errors = addResult.Errors });
            }

            return Ok(new { Message = "User role updated successfully" });
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return Ok(roles);
        }
        [HttpGet("GetUsersWithRoles")]
        public async Task<ActionResult<IEnumerable<UserRoleDTO>>> GetUsersWithRoles()
        {
            var users = userManager.Users.Where(x=>x.OrganizationId == GetCurrentUserAsync().Result.OrganizationId && x.IsDeleted == false).ToList();
            var userRoleViewModels = new List<UserRoleDTO>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userRoleViewModel = new UserRoleDTO
                {
                    UserId = user.Id,
                    UserName = user.FullName,
                    Roles = roles.ToList()
                };

                userRoleViewModels.Add(userRoleViewModel);
            }

            return Ok(userRoleViewModels);
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
    }
}
