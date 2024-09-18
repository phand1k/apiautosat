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
    [Authorize(Roles = "Директор, Администратор, Мастер")]
    [CheckSubscription]
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
        private readonly INotificationCenterService notificationCenterService;
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

        [HttpPatch("DeleteUser")]
        public async Task<IActionResult> DeleteUser([Required]string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("Succes for delete user: "+ id);
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

        [HttpGet("GetAllWashServicesWithPhoneNumber")]
        public async Task<IActionResult> GetAllWashServicesWithPhoneNumber(string? phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }
            var list = await _washService.GetAllWashServicesWithPhoneNumber(phoneNumber);
            return Ok(list);
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
    }
}
