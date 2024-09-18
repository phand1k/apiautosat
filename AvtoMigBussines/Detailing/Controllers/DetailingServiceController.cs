using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Data;
using AvtoMigBussines.Detailing.DetailingDTOModels;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Services.Implementations;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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
        private readonly ApplicationDbContext context;
        private readonly IDetailingOrderService _detailingOrderService;
        private readonly IServiceService _service;
        public DetailingServiceController(UserManager<AspNetUser> userManager, IDetailingServiceService detailingServiceService, ILogger<DetailingServiceController> logger, IDetailingPriceListService detailingPriceListService, ApplicationDbContext context, IDetailingOrderService detailingOrderService, IServiceService _service)
        {
            this.userManager = userManager;
            this.detailingServiceService = detailingServiceService;
            this.logger = logger;
            this.detailingPriceListService = detailingPriceListService;
            this.context = context;
            _detailingOrderService = detailingOrderService;
            this._service = _service;
        }

        private async Task<IEnumerable<string>> GetAllUserTokensAsync(int? organizationId)
        {
            var tokens = await context.NotifiactionTokens
                .Where(x => x.OrganizationId == organizationId)
                .GroupBy(x => x.Token)
                .Select(g => g.OrderByDescending(x => x.DateOfCreated).First().Token)
                .ToListAsync();

            return tokens;
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
                // Получаем информацию о заказе по DetailingOrderId (госномер авто)
                var detailingOrder = await _detailingOrderService.GetDetailingOrderByIdAsync(detailingServiceDTO.DetailingOrderId.Value);
                if (detailingOrder == null)
                {
                    return NotFound(new { Message = "Detailing order not found." });
                }

                // Получаем информацию об услуге по ServiceId (название услуги)
                var service = await _service.GetServiceByIdAsync(detailingServiceDTO.ServiceId.Value);
                if (service == null)
                {
                    return NotFound(new { Message = "Service not found." });
                }

                // Теперь у нас есть госномер автомобиля и название услуги
                detailingServiceDTO.CarNumber = detailingOrder.CarNumber;
                detailingServiceDTO.ServiceName = service.Name;

                // Создаем услугу для детейлинга
                await detailingServiceService.CreateDetailingServiceAsync(detailingServiceDTO, user.Id);
                await detailingPriceListService.CreatePriceListAsync(user.Id, detailingServiceDTO.ServiceId, detailingServiceDTO.DetailingOrderId, detailingServiceDTO.Price);

                // Отправляем уведомления
                var tokens = await GetAllUserTokensAsync(user.OrganizationId);
                foreach (var token in tokens)
                {
                    await SendPushNotification(token, "На авто назначена услуга✅",
                        $"Гос номер: {detailingServiceDTO.CarNumber}",
                        $"Цена: {detailingServiceDTO.Price}\nУслуга: {detailingServiceDTO.ServiceName}",
                        new { extraData = "Любые дополнительные данные" });
                }

                return Ok(detailingServiceDTO);
            }
            catch (CustomException.WashOrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating the wash service.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." + ex.Message });
            }
        }

    }
}
