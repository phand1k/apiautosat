using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Services.Implementations;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Data;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Services.Implementations;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

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
        private readonly ApplicationDbContext context;
        private readonly INotificationCenterService notificationCenterService;
        private readonly IWashOrderTransactionService washOrderTransactionService;
        public DetailingOrderController(UserManager<AspNetUser> userManager, IDetailingOrderService _detailingOrderService, IDetailingServiceService detailingService, ApplicationDbContext context , INotificationCenterService notificationCenterService, IWashOrderTransactionService washOrderTransactionService)
        {
            _userManager = userManager;
            this._detailingOrderService = _detailingOrderService;
            _detailingService = detailingService;
            this.context = context;
            this.notificationCenterService = notificationCenterService;
            this.washOrderTransactionService = washOrderTransactionService;
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
        [HttpPatch("CompleteDetailingOrder")]
        public async Task<IActionResult> CompleteDetailingOrder(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Detailing order ID is required");
            }
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var detailingOrder = await _detailingOrderService.GetByIdDetailingOrderForComplete(id.Value);
            if (detailingOrder == null)
            {
                return NotFound("Detailing order not found");
            }

            bool hasUpdated = await _detailingOrderService.CompleteUpdateDetailingOrderAsync(detailingOrder, user.Id);
            await notificationCenterService.CreateNotificationAsync("Машина: " + detailingOrder.Car.Name + " " + detailingOrder.ModelCar.Name + ". \nГос номер: " + detailingOrder.CarNumber + ". \nНомер клиента: " + detailingOrder.PhoneNumber, user.Id, "Заказ-наряд завершен✅");
            if (hasUpdated)
            {
                return StatusCode(201, "There were incomplete services. Detailing order updated and completed.");
            }
            else
            {
                return StatusCode(200, "All services were already completed. No updates were made.");
            }
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
        [HttpGet("AllCompletedDetailingOrders")]
        public async Task<IActionResult> AllCompletedDetailingOrders(DateTime? dateOfStart, DateTime? dateOfEnd)
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

            var result = await _detailingOrderService.GettAllCompletedDetailingOrdersFilterAsync(user.Id, user.OrganizationId, dateOfStart, dateOfEnd);

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
        [HttpGet("GetInfoPaymentForDetailingOrder")]
        public async Task<IActionResult> GetInfoPaymentForDetailingOrder([Required] int id)
        {
            var payment = await washOrderTransactionService.GetDetailingOrderTransactionByIdAsync(id);
            return Ok(payment);
        }

        [HttpPost("CreateDetailingOrder")]
        public async Task<IActionResult> CreateDetailingOrder([FromBody] DetailingOrder detailingOrder)
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
                await _detailingOrderService.CreateDetailingOrderAsync(detailingOrder, user.Id);
                await notificationCenterService.CreateNotificationAsync($"Создан новый заказ-наряд.\nГос номер: {detailingOrder.CarNumber}", user.Id, "Машина приехала на детейлинг🔧");

                return Ok(detailingOrder);
            }
            catch (CustomException.WashOrderExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while creating the wash order.");
                return StatusCode(500, new { Message = "An error occurred while processing your request." + ex.Message });
            }
        }
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
                await notificationCenterService.CreateNotificationAsync($"Машина приехала на детейлинг🔧. Гос номер: {order.CarNumber}", user.Id, "Создан новый заказ-наряд");
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
