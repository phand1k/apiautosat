using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Data;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace AvtoMigBussines.CarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Директор")]
    public class DirectorController : Controller
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
        public DirectorController(
            IWashOrderService washOrderService,
            WebSocketHandler webSocketHandler,
            IUserService userService,
            UserManager<AspNetUser> userManager,
            IServiceService serviceService,
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context,
            IWashServiceService washService,
            ISalarySettingService salarySettingService,
            IWashOrderTransactionService washOrderTransactionService)
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
        }

        [HttpPatch("DeleteWashServiceFromOrder")]
        public async Task<IActionResult> DeleteWashServiceFromOrder(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var washService = await _washService.GetByIdAsync(id);
            if (washService == null)
            {
                return NotFound(new { Message = "Wash service not found." });
            }

            await _washService.DeleteAsync(washService.Id);
            return Ok(washService);
        }

        [HttpGet("GetAllCompletedWashOrdersAsync")]
        public async Task<IActionResult> GetAllCompletedWashOrdersAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var result = await _washOrderService.GettAllCompletedWashOrdersFilterAsync(user.Id, user.OrganizationId);
            return Ok(result);
        }
        [HttpGet("GetAllNotCompletedWashOrdersAsync")]
        public async Task<IActionResult> GetAllNotCompletedWashOrdersAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var result = await _washOrderService.GetAllWashOrdersNotCompletedFilterAsync(user.Id, user.OrganizationId);
            return Ok(result);
        }
        [HttpGet("GetAllWashOrdersAsync")]
        public async Task<IActionResult> GetAllWashOrdersAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var result = await _washOrderService.GetAllWashOrdersFilterAsync(user.Id, user.OrganizationId);
            return Ok(result);
        }
        [HttpGet("GetAllWashServicesOnOrderAsync")]
        public async Task<IActionResult> GetAllWashServicesOnOrderAsync(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var washServices = await _washService.GetAllWashServicesOnOrder(id, user.Id);
            return Ok(washServices);
        }
        [HttpGet("GetCountOfWashServicesOnOrder")]
        public async Task<IActionResult> GetCountOfWashServicesOnOrder(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var countOfWashServices = await _washService.GetCountAllServices(id);
            return Ok(countOfWashServices);
        }
        [HttpPatch("DeleteUser")]
        public async Task<IActionResult> DeleteUser([Required]string id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            await _userService.DeleteUserAsync(id);
            return Ok("Succes for delete user: "+ id);
        }
        [HttpGet("GetSummOfWashServicesOnOrder")]
        public async Task<IActionResult> GetSummOfWashServicesOnOrder(int? id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not auth" });
            }
            var summOfWashServices = await _washService.GetSummAllServices(id);
            return Ok(summOfWashServices);
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var users = await _userService.GetAllUsersAsync(user.OrganizationId);
            return Ok(users);
        }
        [HttpGet("GetDetailsWashService")]
        public async Task<IActionResult> GetDetailsWashService([Required] int id)
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
        [HttpPatch("DeleteWashOrder")]
        public async Task<IActionResult> DeleteWashOrder(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("WashOrder ID is required");
            }
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var washOrder = await _washOrderService.GetWashOrderByIdAsync(id.Value);
            if (washOrder == null)
            {
                return NotFound("WashOrder not found");
            }
            await _washOrderService.DeleteUpdateWashOrderAsync(washOrder);
            return Ok("Wash order success deleted");
        }
        [HttpPatch("CompleteWashOrder")]
        public async Task<IActionResult> CompleteWashOrder(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("WashOrder ID is required");
            }
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var washOrder = await _washOrderService.GetWashOrderByIdAsync(id.Value);
            if (washOrder == null)
            {
                return NotFound("WashOrder not found");
            }

            bool hasUpdated = await _washOrderService.CompleteUpdateWashOrderAsync(washOrder, user.Id);

            if (hasUpdated)
            {
                return StatusCode(201, "There were incomplete services. Wash order updated and completed.");
            }
            else
            {
                return StatusCode(200, "All services were already completed. No updates were made.");
            }
        }
        [HttpPost("CreateWashOrderTransactionAsync")]
        public async Task<IActionResult> CreateWashOrderTransactionAsync([FromBody] WashOrderTransaction transaction, int washOrderId)
        {
            if (washOrderId == null)
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
                await washOrderTransactionService.CreateWashOrderTransactionAsync(transaction, user.Id, washOrderId);
                return Ok(transaction);
            }
            catch (CustomException.WashOrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Логирование исключения
                // _logger.LogError(ex, "An error occurred while creating the wash service.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }
        [HttpGet("GetInfoPaymentForWashOrder")]
        public async Task<IActionResult> GetInfoPaymentForWashOrder([Required] int id)
        {
            var payment = await washOrderTransactionService.GetWashOrderTransactionByIdAsync(id);
            return Ok(payment);
        }
        [HttpGet("GetInfoForWashorderList")]
        public async Task<IActionResult> GetInfoForWashorderList()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            WashOrderDashboardDTO washOrderDashboardDTO = new WashOrderDashboardDTO();
            washOrderDashboardDTO.CountOfNotCompletedOrders = await _washOrderService.GetCountOfNotCompletedWashOrdersAsync(user.Id, user.OrganizationId);
            return Ok(washOrderDashboardDTO);
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
                var token = await GetSpecificToken(washServiceDTO.WhomAspNetUserId);
                await SendPushNotification(token, "На вас назначена услуга:", $"На вас назначена услуга: {washServiceDTO.ServiceName}, цена: {washServiceDTO.Price}",
                    new { extraData = "Любые дополнительные данные" });
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
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }
        [HttpGet("GetSalaryUser")]
        public async Task<IActionResult> GetSalaryUser(int? serviceId, string? aspNetUserId)
        {
            if (serviceId == null || aspNetUserId == null)
            {
                return BadRequest(new { Message = "Service ID and User ID are required." });
            }

            var salary = await salarySettingService.GetSalarySettingForUser(aspNetUserId, serviceId);
            if (salary == null)
            {
                return NotFound(new { Message = "Salary for user not found." });
            }

            return Ok(salary.Salary);
        }

        [HttpPost("CreateSalarySetting")]
        public async Task<IActionResult> CreateSalarySetting([FromBody] SalarySettingDTO salarySettingDTO)
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
                await salarySettingService.CreateSalarySettingAsync(salarySettingDTO);
                return Ok(salarySettingDTO);
            }
            catch (CustomException.WashOrderExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while creating the wash order.");
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
                var tokens = await GetAllUserTokensAsync(user.OrganizationId); // Метод для получения всех токенов пользователей
                foreach (var token in tokens)
                {
                    await SendPushNotification(token, "Создана новая услуга", $"Название услуги: {service.Name}, цена: {service.Price}", new { extraData = "Любые дополнительные данные" });
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
        [HttpGet("GetDetailsWashOrder")]
        public async Task<IActionResult> GetDetailsWashOrder([Required] int id)
        {
            var detailsWashOrder = await _washOrderService.GetWashOrderByIdAsync(id);
            return Ok(detailsWashOrder);
        }
        [HttpPost("CreateWashOrder")]
        public async Task<IActionResult> CreateWashOrder([FromBody] WashOrder washOrder)
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
                await _washOrderService.CreateWashOrderAsync(washOrder, user.Id);

                var tokens = await GetAllUserTokensAsync(user.OrganizationId);
                foreach (var token in tokens)
                {
                    await SendPushNotification(token, "Создан новый заказ-наряд", $"Детали: {washOrder.CarNumber}, цена: {washOrder.CarNumber}", new { extraData = "Любые дополнительные данные" });
                }

                return Ok(washOrder);
            }
            catch (CustomException.WashOrderExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while creating the wash order.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
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
    }
}
