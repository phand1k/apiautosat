using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AvtoMigBussines.CarWash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [CheckSubscription]
    public class WashOrderReportController : Controller
    {
        private readonly IWashOrderService _washOrderService;
        private readonly IUserService _userService;
        private readonly UserManager<AspNetUser> _userManager;
        public WashOrderReportController(IWashOrderService washOrderService, IUserService userService, UserManager<AspNetUser> userManager)
        {
            _washOrderService = washOrderService;
            _userService = userService;
            _userManager = userManager;
        }
    }
}
