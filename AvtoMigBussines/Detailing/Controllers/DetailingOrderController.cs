using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Services.Implementations;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Services.Implementations;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AvtoMigBussines.Detailing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CheckSubscription]
    public class DetailingOrderController : Controller
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IDetailingOrderService _detailingOrderService;
        private readonly IDetailingServiceService _detailingService;
        public DetailingOrderController(UserManager<AspNetUser> userManager, IDetailingOrderService _detailingOrderService, IDetailingServiceService detailingService)
        {
            _userManager = userManager;
            this._detailingOrderService = _detailingOrderService;
            _detailingService = detailingService;
        }

        private async Task<AspNetUser> GetCurrentUserAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }

            var aspNetUser = await _userManager.FindByEmailAsync(userName);
            if (aspNetUser == null)
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(aspNetUser.Id);
            return user;
        }
        [HttpPatch("DeleteDetailingOrder")]
        public async Task<IActionResult> DeleteDetailingOrder([Required] int id)
        {
            var detailingOrder = await _detailingOrderService.GetDetailingOrderByIdAsync(id);
            if (detailingOrder == null)
            {
                return NotFound("Detailing Order not found");
            }
            await _detailingOrderService.DeleteUpdateDetailingOrderAsync(detailingOrder);
            return Ok();
        }
        [HttpGet("DetailsDetailingOrder")]
        public async Task<IActionResult> DetailsDetailingOrder([Required] int id)
        {
            var detailsWashOrder = await _detailingOrderService.GetDetailingOrderByIdAsync(id);
            return Ok(detailsWashOrder);
        }
        [HttpGet("GetSummOfDetailingServicesOnOrder")]
        public async Task<IActionResult> GetSummOfDetailingServicesOnOrder(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var summOfWashServices = await _detailingService.GetSummAllServices(id);
            return Ok(summOfWashServices);
        }
        [HttpGet("AllNotCompletedOrders")]
        public async Task<IActionResult> AllNotCompletedOrders()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var result = await _detailingOrderService.GetAllOrdersNotCompletedFilterAsync(user.Id, user.OrganizationId);
            return Ok(result);
        }

        /*[HttpGet("GetSummOfDetailingServicesOnOrder")]
        public async Task<IActionResult> GetSummOfWashServicesOnOrder(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var summOfWashServices = await _washService.GetSummAllServices(id);
            return Ok(summOfWashServices); ПОЛУЧЕНИЕ СУММЫ ДЛЯ ЗАКАЗ-НАРЯДА
        }*/

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] DetailingOrder order)
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
                await _detailingOrderService.CreateDetailingOrderAsync(order, user.Id);


                return Ok(order);
            }
            catch (CustomException.WashOrderExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request." + ex.Message });
            }
        }
    }
}
