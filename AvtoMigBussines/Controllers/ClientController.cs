using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AvtoMigBussines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IClientRepository _clientRepository;
        public ClientController(IClientService _clientService, IClientRepository _clientRepository)
        {
            this._clientService = _clientService;
            this._clientRepository = _clientRepository;
        }
        [HttpPost("SubscripeToUpdates")]
        public async Task<IActionResult> SubscripeToUpdates(string? carNumber, long userId)
        {
            if (carNumber == null || userId == null)
            {
                return BadRequest();
            }
            await _clientService.SubscribeToUpdatesAsync(carNumber, userId);
            return Ok("Success to subscripe");
        }

    }
}
