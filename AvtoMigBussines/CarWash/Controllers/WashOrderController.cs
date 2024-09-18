using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Services.Implementations;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Data;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AvtoMigBussines.CarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [CheckSubscription]
    public class WashOrderController : Controller
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
        private readonly IClientService _clientService;
        //1ekkawekwk
        public WashOrderController(
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
            INotificationCenterService notificationCenterService,
            IClientService clientService)
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
            _clientService = clientService;
        }
        //Actions with push notification, get current user etc
        //Действия с получением текущего пользователя, отправка уведомлений и т.д.
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
        private async Task SendPushNotification(string deviceToken, string title, string subtitle, string body, object data)
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // временное решение для отладки
            };

            // Ensure HTTP/2
            clientHandler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            clientHandler.MaxConnectionsPerServer = 10;

            var client = new HttpClient(clientHandler)
            {
                DefaultRequestVersion = new Version(2, 0), // HTTP/2
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };

            var url = $"https://api.push.apple.com/3/device/{deviceToken}";

            var requestContent = new
            {
                aps = new
                {
                    alert = new
                    {
                        title = title,
                        subtitle = subtitle,
                        body = body
                    },
                    sound = "default"
                },
                customData = data
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");

            var teamId = "J34Q6XL84S"; // Замените на ваш teamId
            var keyId = "JWK76Z48GJ"; // Замените на ваш keyId
            var privateKey = "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgynNA+11haalnht82\r\nhdH/ZE6YOL4raeejSUawSibLPfGgCgYIKoZIzj0DAQehRANCAAT5FE8RDWRZHNY1\r\nqB6IPcqsPWTZlVhGuuUeLZbhoCxSDhLcUh0vSIrNo4Ewk7Awab0EBKVOBy0e/fLn\r\nhAuEFB+d";

            var jwtToken = JwtTokenGenerator.GenerateJwtToken(teamId, keyId, privateKey);
            Console.WriteLine($"Generated JWT: {jwtToken}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", jwtToken);
            client.DefaultRequestHeaders.Add("apns-topic", "com.phand1k.AvtoMig"); // Проверьте идентификатор пакета
            client.DefaultRequestHeaders.Add("apns-push-type", "alert");
            client.DefaultRequestHeaders.Add("apns-priority", "10");
            client.DefaultRequestHeaders.Add("apns-expiration", "0");

            try
            {
                var response = await client.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error sending push notification: {response.StatusCode} - {responseContent}");
                    throw new Exception($"Error sending push notification: {response.StatusCode} - {responseContent}");
                }
                Console.WriteLine("Notification sent successfully.");
            }
            catch (HttpRequestException httpRequestException)
            {
                Console.WriteLine($"HttpRequestException: {httpRequestException.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
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


        //Actions with Order
        //Действия с заказ-нарядами
        [HttpPatch("ReturnOrder")]
        public async Task<IActionResult> ReturnOrder([Required] int id)
        {
            await _washOrderService.ReturnWashOrderAsync(id);
            return Ok();
        }

        [HttpGet("GetInfoPaymentForWashOrder")]
        public async Task<IActionResult> GetInfoPaymentForWashOrder([Required] int id)
        {
            var payment = await washOrderTransactionService.GetWashOrderTransactionByIdAsync(id);
            return Ok(payment);
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

        [HttpGet("GetInfoForWashorderList")]
        public async Task<IActionResult> GetInfoForWashorderList()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            WashOrderDashboardDTO washOrderDashboardDTO = new WashOrderDashboardDTO();
            washOrderDashboardDTO.CountOfNotCompletedServices = await _washService.GetCountOfNotCompletedServicesOnNotCompletedOrders(user.OrganizationId);
            washOrderDashboardDTO.CountOfCompeltedServices = await _washService.GetCountOfCompletedServicesOnNotCompletedOrders(user.OrganizationId);
            washOrderDashboardDTO.CountOfNotCompletedOrders = await _washOrderService.GetCountOfNotCompletedWashOrdersAsync(user.Id, user.OrganizationId);
            washOrderDashboardDTO.SummOfAllServices = await _washService.GetSummOfServicesOnNotCompletedWashOrders(user.OrganizationId);
            return Ok(washOrderDashboardDTO);
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

        [HttpGet("AllCompletedWashOrdersAsync")]
        public async Task<IActionResult> AllCompletedWashOrders(DateTime? dateOfStart, DateTime? dateOfEnd)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            // Если даты не переданы, то использовать текущий день с 05:00 до 23:59
            if (!dateOfStart.HasValue && !dateOfEnd.HasValue)
            {
                DateTime today = DateTime.Today;
                dateOfStart = today.AddHours(5);  // Сегодня в 05:00
                dateOfEnd = today.AddHours(23).AddMinutes(59).AddSeconds(59);  // Сегодня в 23:59:59
            }

            var result = await _washOrderService.GettAllCompletedWashOrdersFilterAsync(user.Id, user.OrganizationId, dateOfStart, dateOfEnd);

            var response = result.Select(order => new
            {
                order.CarNumber,
                order.AspNetUserId,
                AspNetUser = new
                {
                    order.AspNetUser?.FirstName,
                    order.AspNetUser?.LastName,
                    order.AspNetUser?.PhoneNumber
                },
                order.OrganizationId,
                Organization = new
                {
                    order.Organization?.Name,
                    order.Organization?.FullName
                },
                order.EndOfOrderAspNetUserId,
                EndOfOrderAspNetUser = new
                {
                    order.EndOfOrderAspNetUser?.FirstName,
                    order.EndOfOrderAspNetUser?.LastName,
                    order.EndOfOrderAspNetUser?.PhoneNumber
                },
                order.CreatedByFullName,
                order.EndedByFullName,
                order.Id,
                order.DateOfCreated,
                order.CarId,
                Car = new
                {
                    order.Car?.Name
                },
                order.ModelCarId,
                ModelCar = new
                {
                    order.ModelCar?.Name
                },
                order.IsDeleted,
                order.IsReturn,
                order.IsOvered,
                order.DateOfCompleteService,
                order.PhoneNumber
            });

            return Ok(response);
        }



        [HttpGet("AllNotCompletedWashOrdersAsync")]
        public async Task<IActionResult> AllNotCompletedWashOrdersAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            var result = await _washOrderService.GetAllWashOrdersNotCompletedFilterAsync(user.Id, user.OrganizationId);
            return Ok(result);
        }

        [HttpGet("AllWashOrdersAsync")]
        public async Task<IActionResult> AllWashOrdersAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var result = await _washOrderService.GetAllWashOrdersFilterAsync(user.Id, user.OrganizationId);
            return Ok(result);
        }
        [HttpPatch("DeleteWashOrder")]
        public async Task<IActionResult> DeleteWashOrder(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("WashOrder ID is required");
            }
            var washOrder = await _washOrderService.GetByIdWashOrderForComplete(id.Value);
            if (washOrder == null)
            {
                return NotFound("WashOrder not found");
            }
            await _washOrderService.DeleteUpdateWashOrderAsync(washOrder);
            return Ok("Wash order success deleted");
        }
        [HttpPatch("ReadyWashOrder")]
        public async Task<IActionResult> ReadyWashOrder(int? id)
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

            var washOrder = await _washOrderService.GetByIdWashOrderForComplete(id.Value);
            if (washOrder == null)
            {
                return NotFound("WashOrder not found");
            }

            await _washOrderService.ReadyUpdateWashOrderAsync(washOrder);

            Console.WriteLine("ReadyUpdateWashOrderAsync выполнен успешно");

            // Отправка уведомлений через Telegram
            await _clientService.NotifyUsersAsync(washOrder.CarNumber);
            Console.WriteLine($"NotifyUsersAsync вызван для номера машины: {washOrder.CarNumber}");
            var tokens = await GetAllUserTokensAsync(user.OrganizationId);
            foreach (var token in tokens)
            {
                await SendPushNotification(token, "Машина помыта✅", $"Гос номер: {washOrder.CarNumber}", $"Машина: {washOrder.Car.Name + " " + washOrder.ModelCar.Name}", new { extraData = "Любые дополнительные данные" });
            }
            await notificationCenterService.CreateNotificationAsync("Машина: " + washOrder.Car.Name + " " + washOrder.ModelCar.Name + ". \nГос номер: " + washOrder.CarNumber + ". \nНомер клиента: " + washOrder.PhoneNumber, user.Id, "Заказ-наряд завершен");

            return StatusCode(200, "Wash order is ready");
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
            var washOrder = await _washOrderService.GetByIdWashOrderForComplete(id.Value);
            if (washOrder == null)
            {
                return NotFound("WashOrder not found");
            }

            bool hasUpdated = await _washOrderService.CompleteUpdateWashOrderAsync(washOrder, user.Id);
            var tokens = await GetAllUserTokensAsync(user.OrganizationId);
            foreach (var token in tokens)
            {
                await SendPushNotification(token, "Машина помыта✅", $"Гос номер: {washOrder.CarNumber}", $"Машина: {washOrder.Car.Name + " " + washOrder.ModelCar.Name}", new { extraData = "Любые дополнительные данные" });
            }
            await notificationCenterService.CreateNotificationAsync("Машина: " + washOrder.Car.Name + " " + washOrder.ModelCar.Name + ". \nГос номер: " + washOrder.CarNumber + ". \nНомер клиента: " + washOrder.PhoneNumber, user.Id, "Заказ-наряд завершен");

            if (hasUpdated)
            {
                return StatusCode(201, "There were incomplete services. Wash order updated and completed.");
            }
            else
            {
                return StatusCode(200, "All services were already completed. No updates were made.");
            }
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
                var washOrderDetails = await _washOrderService.GetWashOrderByIdAsync(washOrder.Id);

                var tokens = await GetAllUserTokensAsync(user.OrganizationId);
                foreach (var token in tokens)
                {
                    await SendPushNotification(token, "Машина приехала на мойку🌊", $"Гос номер: {washOrder.CarNumber}", $"Номер клиента: {washOrder.PhoneNumber}", new { extraData = "Не забудьте назначить услугу на заказ-наряд" });
                }
                await notificationCenterService.CreateNotificationAsync($"Создан новый заказ-наряд. Гос номер: {washOrder.CarNumber}", user.Id, "Создан новый заказ-наряд");

                var message = JsonConvert.SerializeObject(new
                {
                    eventType = "create",
                    order = new
                    {
                        id = washOrder.Id,
                        carNumber = washOrder.CarNumber,
                        phoneNumber = washOrder.PhoneNumber,
                        dateOfCreated = washOrder.DateOfCreated,
                        name = washOrder.Car?.Name +" "+ washOrder.ModelCar?.Name
                        // добавляем дату создания для использования на клиенте
                    }
                });
                await _webSocketHandler.SendMessageToAllAsync(message);

                return Ok(washOrder);
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


        [HttpGet("DetailsWashOrder")]
        public async Task<IActionResult> DetailsWashOrder([Required] int id)
        {
            var detailsWashOrder = await _washOrderService.GetWashOrderByIdAsync(id);
            return Ok(detailsWashOrder);
        }
    }
}
