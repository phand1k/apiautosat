using AvtoMigBussines.Authenticate.Models;
using AvtoMigBussines.Data;
using AvtoMigBussines.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private ApplicationDbContext _context;
        private RoleManager<IdentityRole> roleManager;
        public RoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.roleManager = roleManager;
        }
        [HttpGet]
        [Route("UserRoles")]
        public async Task<IActionResult> UserRoles()
        {
            var list = await _context.UserRoles.ToListAsync();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        [HttpGet]
        [Route("AspNetRoles")]
        public async Task<IActionResult> AspNetRoles()
        {
            var list = await _context.Roles.ToListAsync();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }
        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody][Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    // В REST API предпочтительнее возвращать статус 201 Created для успешно созданных ресурсов,
                    // но так как мы не создаем конкретный URI для роли, можно вернуть 200 OK.
                    // Для создания ресурса с URI можно использовать CreatedAtRoute или CreatedAtAction.
                    return Ok(201); // Или CreatedAtAction("GetRole", new { name = name }, null);
                }
                else
                {
                    // Возвращаем список ошибок, если не удалось создать роль
                    return BadRequest(result.Errors);
                }
            }

            // Если модель не проходит валидацию
            return BadRequest(ModelState);
        }

    }
}
