using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Data;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace AvtoMigBussines.CarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [CheckSubscription]
    public class WashServiceController : Controller
    {
        private readonly IWashOrderService _washOrderService;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly IUserService _userService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IServiceService _serviceService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;
        private readonly IWashServiceService _washService;
        private readonly ISalarySettingService salarySettingService;
        private readonly IWashOrderTransactionService washOrderTransactionService;
        private readonly INotificationCenterService notificationCenterService;
        public WashServiceController(
            IWashOrderService washOrderService,
            WebSocketHandler webSocketHandler,
            IUserService userService,
            UserManager<AspNetUser> userManager,
            IServiceService serviceService,
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context,
            IWashServiceService washService,
            ISalarySettingService salarySettingService,
            IWashOrderTransactionService washOrderTransactionService,
            INotificationCenterService notificationCenterService)
        {
            _washOrderService = washOrderService;
            _webSocketHandler = webSocketHandler;
            _userService = userService;
            _userManager = userManager;
            _serviceService = serviceService;
            _httpClientFactory = httpClientFactory;
            _context = context;
            _washService = washService;
            this.salarySettingService = salarySettingService;
            this.washOrderTransactionService = washOrderTransactionService;
            this.notificationCenterService = notificationCenterService;
        }

        private async Task<string?> GetSpecificToken(string? aspNetUserId)
        {
            var token = await _context.NotifiactionTokens
                .Where(x => x.AspNetUserId == aspNetUserId)
                .OrderByDescending(x => x.DateOfCreated)
                .Select(x => x.Token)
                .FirstOrDefaultAsync();

            return token;
        }
        private async Task SendPushNotification(string token, string title, string body, object data)
        {
            var client = _httpClientFactory.CreateClient();
            var requestContent = new
            {
                to = token,
                title = title,
                body = body,
                data = data,
                sound = "default",
                priority = "normal"
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://exp.host/--/api/v2/push/send", content);
            response.EnsureSuccessStatusCode();
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
        [HttpPatch("DeleteWashServiceFromOrder")]
        public async Task<IActionResult> DeleteWashServiceFromOrder(int id)
        {
            var washService = await _washService.GetByIdAsync(id);
            if (washService == null)
            {
                return NotFound(new { Message = "Wash service not found." });
            }

            var washOrderId = washService.WashOrderId; // Предполагается, что у услуги есть WashOrderId
            await _washService.DeleteAsync(washService.Id);

            // Отправка сообщения по веб-сокету
            var message = JsonConvert.SerializeObject(new
            {
                eventType = "serviceDeleted",
                order = new { id = washOrderId }
            });
            await _webSocketHandler.SendMessageToAllAsync(message);

            return Ok(washService);
        }

        [HttpGet("AllWashServicesOnOrderAsync")]
        public async Task<IActionResult> AllWashServicesOnOrderAsync(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var washServices = await _washService.GetAllWashServicesOnOrder(id, user.Id);
            return Ok(washServices);
        }

        [HttpGet("DetailsWashService")]
        public async Task<IActionResult> DetailsWashService([Required] int id)
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

            return Ok(washService);
        }
        [HttpPost("CreateWashService")]
        public async Task<IActionResult> CreateWashService([FromBody] WashServiceDTO washServiceDTO)
        {
            if (washServiceDTO.WashOrderId == null)
            {
                return BadRequest(new { Message = "Wash order ID is required." });
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
                await _washService.CreateAsync(washServiceDTO, user.Id);

                // Получаем обновленную сумму услуг
                var newTotalServices = await _washService.GetSummAllServices(washServiceDTO.WashOrderId);

                // Формируем сообщение для веб-сокета
                var message = JsonConvert.SerializeObject(new
                {
                    eventType = "serviceUpdated",
                    orderId = washServiceDTO.WashOrderId,
                    newTotalServices = newTotalServices
                });

                // Отправляем сообщение всем клиентам
                await _webSocketHandler.SendMessageToAllAsync(message);

                return Ok(washServiceDTO);
            }
            catch (CustomException.WashOrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Логирование исключения
                // _logger.LogError(ex, "An error occurred while creating the wash service.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." + ex.Message });
            }
        }


    }
}
