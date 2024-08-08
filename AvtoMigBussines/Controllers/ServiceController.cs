using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Data;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [CheckSubscription]
    public class ServiceController : Controller
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
        public ServiceController(
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

        private async Task<IEnumerable<string>> GetAllUserTokensAsync(int? organizationId)
        {
            var tokens = await _context.NotifiactionTokens
                .Where(x => x.OrganizationId == organizationId)
                .GroupBy(x => x.Token)
                .Select(g => g.OrderByDescending(x => x.DateOfCreated).First().Token)
                .ToListAsync();

            return tokens;
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


        [HttpPost("DeleteService")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound(new { Message = "Service not found." });
            }

            await _serviceService.DeleteServiceAsync(service.Id);
            return Ok(service);
        }
        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService([FromBody] Service service)
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
                await _serviceService.CreateServiceAsync(service, user.Id);

                var message = JsonConvert.SerializeObject(new { name = service.Name, price = service.Price });
                var tokens = await GetAllUserTokensAsync(user.OrganizationId); // Метод для получения всех токенов для уведомлений в приложении пользователей
                foreach (var token in tokens)
                {
                    await SendPushNotification(token, "Создана новая услуга⚙️", $"Название услуги: {service.Name},\nцена: {service.Price}", new { extraData = "Любые дополнительные данные" });
                }

                await _webSocketHandler.SendMessageToAllAsync(message);

                return Ok(service);
            }
            catch (CustomException.ServiceExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while creating the service.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }


        [HttpGet("GetAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var result = await _serviceService.GetAllServicesAsync(user.Id);
            return Ok(result);
        }

    }
}
