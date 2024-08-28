using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.DetailingDTOModels;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AvtoMigBussines.Detailing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DetailingServiceController : Controller
    {
        private readonly UserManager<AspNetUser> userManager;
        private readonly IDetailingServiceService detailingServiceService;
        private readonly ILogger<DetailingServiceController> logger;
        private readonly IDetailingPriceListService detailingPriceListService;
        public DetailingServiceController(UserManager<AspNetUser> userManager, IDetailingServiceService detailingServiceService, ILogger<DetailingServiceController> logger, IDetailingPriceListService detailingPriceListService)
        {
            this.userManager = userManager;
            this.detailingServiceService = detailingServiceService;
            this.logger = logger;
            this.detailingPriceListService = detailingPriceListService;
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
        [HttpGet("PriceListForService")]
        public async Task<IActionResult> PriceListForService(int? serviceId, int? carId, int? modelCarId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var result = await detailingPriceListService.GetAllServices(serviceId, carId, modelCarId, user.OrganizationId);
            return Ok(result);
        }
        [HttpPatch("DeleteWashServiceFromOrder")]
        public async Task<IActionResult> DeleteWashServiceFromOrder(int id)
        {
            var detailingService = await detailingServiceService.GetDetailingServiceByIdAsync(id);
            if (detailingService == null)
            {
                return NotFound(new { Message = "Wash service not found." });
            }

            await detailingServiceService.DeleteDetailingServiceAsync(detailingService.Id);

            // Отправка сообщения по веб-сокету

            return Ok(detailingServiceService);
        }
        [HttpGet("AllDetailingServicesOnOrderAsync")]
        public async Task<IActionResult> AllDetailingServicesOnOrderAsync(int? id)
        {
            var user = await GetCurrentUserAsync();

            var detailingServices = await detailingServiceService.GetAllDetailingServicesOnOrder(id, user.Id);
            return Ok(detailingServices);
        }
        [HttpPost("CreateDetailingService")]
        public async Task<IActionResult> CreateDetailingService([FromBody] DetailingServiceDTO detailingServiceDTO)
        {
            if (detailingServiceDTO.DetailingOrderId == null)
            {
                return BadRequest(new { Message = "Detailing order ID is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            try
            {
                await detailingServiceService.CreateDetailingServiceAsync(detailingServiceDTO, user.Id);

                await detailingPriceListService.CreatePriceListAsync(user.Id, detailingServiceDTO.ServiceId, detailingServiceDTO.DetailingOrderId, detailingServiceDTO.Price);
                // Получаем обновленную сумму услуг
                //var newTotalServices = await _washService.GetSummAllServices(washServiceDTO.WashOrderId);

                // Формируем сообщение для веб-сокета
                /*var message = JsonConvert.SerializeObject(new
                {
                    eventType = "serviceUpdated",
                    orderId = washServiceDTO.WashOrderId,
                    newTotalServices = newTotalServices
                });*/

                // Отправляем сообщение всем клиентам
                //await _webSocketHandler.SendMessageToAllAsync(message);

                return Ok(detailingServiceDTO);
            }
            catch (CustomException.WashOrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Логирование исключения
                logger.LogError(ex, "An error occurred while creating the wash service.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." + ex.Message });
            }
        }
    }
}
