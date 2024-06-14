using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AvtoMigBussines.Services.Implementations
{
    public class ITokenService : Controller
    {
        private readonly UserManager<AspNetUser> userManager;
        public ITokenService(UserManager<AspNetUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<AspNetUser> GetUserFromToken(string token)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await userManager.FindByNameAsync(userName);
            return user;
        }
    }
}
