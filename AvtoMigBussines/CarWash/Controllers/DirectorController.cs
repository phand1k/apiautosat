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
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace AvtoMigBussines.CarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DirectorController : Controller
    {
        private readonly IWashOrderService _washOrderService;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly IUserService userService;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IServiceService serviceService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext context;
        private readonly IWashServiceService _washService;
        public DirectorController(IWashOrderService washOrderService, WebSocketHandler webSocketHandler, 
            IUserService userService, UserManager<AspNetUser> userManager, IServiceService serviceService, IHttpClientFactory httpClientFactory, ApplicationDbContext context, IWashServiceService washService)
        {
            _washOrderService = washOrderService;
            _webSocketHandler = webSocketHandler;
            this.userService = userService;
            this.userManager = userManager;
            this.serviceService = serviceService;
            _httpClientFactory = httpClientFactory;
            this.context = context;
            _washService = washService;
        }
        [Route("GetAllWashOrdersAsync")]
        [HttpGet]
        public async Task<IActionResult> GetAllWashOrdersAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var result = await _washOrderService.GetAllWashOrdersFilterAsync(user.Id, user.OrganizationId);
                return Ok(result);
            }
            return Unauthorized();
        }
        [Route("GetAllUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var users = await userService.GetAllUsersAsync(user.OrganizationId);
                return Ok(users);
            }
            return Unauthorized();
        }
        [Route("CreateWashService")]
        [HttpPost]
        public async Task<IActionResult> CreateWashService([FromBody] WashServiceDTO washServiceDTO)
        {
            // Проверка на наличие необходимых данных в DTO
            if (washServiceDTO.WashOrderId == null)
            {
                return NotFound(new { Message = "Wash order ID is required." });
            }

            // Проверка модели до выполнения основной логики
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Получение имени пользователя из токена
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            // Получение пользователя по номеру телефона
            var aspNetUser = await userService.GetUserByPhoneNumberAsync(userName);
            if (aspNetUser == null)
            {
                return NotFound(new { Message = "User not found by phone number." });
            }

            // Поиск пользователя в системе по его Id
            var user = await userManager.FindByIdAsync(aspNetUser.Id);
            if (user == null)
            {
                return Unauthorized(new { Message = "User not found in the system." });
            }

            // Основная логика создания WashService
            try
            {
                await _washService.CreateAsync(washServiceDTO, user.Id);
                return Ok(washServiceDTO);
            }
            catch (CustomException.WashOrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Логирование исключения
                // logger.LogError(ex, "An error occurred while creating the wash service."); 
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        [Route("GetAllServices")]
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var result = await serviceService.GetAllServicesAsync(user.Id);
                return Ok(result);
            }
            return Unauthorized();
        }
        [Route("DeleteService")]
        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var service = await serviceService.GetServiceByIdAsync(id);
                if (service == null)
                {
                    return NotFound();
                }
                await serviceService.DeleteServiceAsync(service.Id);
                return Ok(service);
            }
            return Unauthorized();
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

        [Route("CreateService")]
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] Service service)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        await serviceService.CreateServiceAsync(service, user.Id);

                        // Формируем JSON-строку с данными из объекта service
                        var message = JsonConvert.SerializeObject(new { name = service.Name, price = service.Price });
                        var tokens = await GetAllUserTokensAsync(); // Метод для получения всех токенов пользователей
                        foreach (var token in tokens)
                        {
                            await SendPushNotification(token, "Создана новая услуга", "Название услуги:"+service.Name+", цена: "+service.Price, new { extraData = "Любые дополнительные данные" });
                        }

                        await _webSocketHandler.SendMessageToAllAsync(message);

                        return Ok(service);
                    }
                    catch (CustomException.ServiceExistsException ex)
                    {
                        return StatusCode(400, new { Message = ex.Message });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Message = ex.Message });
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }
        private async Task<IEnumerable<string>> GetAllUserTokensAsync()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);

            var tokens = await context.NotifiactionTokens
                .Where(x => x.OrganizationId == user.OrganizationId && x.AspNetUserId == user.Id)
                .GroupBy(x => x.Token)
                .Select(g => g.OrderByDescending(x => x.DateOfCreated).First().Token)
                .ToListAsync();

            return tokens;
        }
        [Route("CreateWashOrder")]
        [HttpPost]
        public async Task<IActionResult> CreateWashOrder([FromBody] WashOrder washOrder)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var aspNetUser = await userService.GetUserByPhoneNumberAsync(userName);
            var user = await userManager.FindByIdAsync(aspNetUser.Id);

            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _washOrderService.CreateWashOrderAsync(washOrder, user.Id);
                        var tokens = await GetAllUserTokensAsync(); // Метод для получения всех токенов пользователей
                        foreach (var token in tokens)
                        {
                            await SendPushNotification(token, "Создан новый заказ-наряд", "Детали:" + washOrder.CarNumber + ", цена: " + washOrder.CarNumber, new { extraData = "Любые дополнительные данные" });
                        }
                        return Ok(washOrder);
                    }
                    catch (CustomException.WashOrderExistsException ex)
                    {
                        return StatusCode(400, new { Message = ex.Message });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Message = ex.Message });
                    }
                }
                return BadRequest();
            }
            return Unauthorized();
        }
    }
}
