using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Authenticate.Models;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AvtoMigBussines.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IConfiguration _configuration;
        private readonly IOrganizationRepository organizationRepository;
        public UserService(IUserRepository userRepository, UserManager<AspNetUser> userManager, IConfiguration configuration, IOrganizationRepository organizationRepository)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            this._configuration = configuration;
            this.organizationRepository = organizationRepository;
        }
        public async Task<AspNetUser> GetUserByIdAsync(string id)
        {
            return await userRepository.GetByIdAsync(id);
        }
        public async Task<AspNetUser> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await userRepository.GetByPhoneNumberAsync(phoneNumber);
        }
        public async Task<IEnumerable<AspNetUser>> GetAllUsersAsync(int? organizationId)
        {
            return await userRepository.GetAllAsync(organizationId);
        }

        public async Task<bool> CreateUserAsync(AspNetUser aspNetUser)
        {
            // Проверяем, существует ли уже пользователь с таким номером телефона
            if (await userRepository.ExistsWithPhoneNumber(aspNetUser.PhoneNumber))
            {
                throw new Exception("User with the same phone number already exists.");
            }

            await userRepository.AddAsync(aspNetUser);
            return true;
        }

        public async Task UpdateUserAsync(AspNetUser user)
        {
            await userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(string id)
        {
            await userRepository.DeleteAsync(id);
        }
        public async Task RegisterUserAsync(RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.PhoneNumber);
            if (userExists != null)
            {
                throw new CustomException.UserAlreadyExistsException("User already exists");
            }
            AspNetUser aspNetUser = new AspNetUser()
            {
                Email = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.PhoneNumber,
                PhoneNumber = model.PhoneNumber,
                NormalizedUserName = model.PhoneNumber
            };
            var checkOrganizationExists = await organizationRepository.GetByNumberAsync(model.OrganizationId);
            if (checkOrganizationExists == null)
            {
                throw new CustomException.OrganizationNotFoundException("Organization with this number does not exist.");
            }
            aspNetUser.OrganizationId = checkOrganizationExists.Id;
            var result = await userManager.CreateAsync(aspNetUser, model.Password);
        }
        public async Task<string> LoginUserAsync(LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.PhoneNumber);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                throw new ArgumentException("Invalid username or password.");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            if (user.OrganizationId.HasValue)
            {
                authClaims.Add(new Claim("OrganizationId", user.OrganizationId.Value.ToString()));
            }

            var token = GetToken(authClaims, user.Id, user.OrganizationId);
            return token;
        }
        private string GetToken(List<Claim> authClaims, string userId, int? organizationId)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            authClaims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

            if (organizationId.HasValue)
            {
                authClaims.Add(new Claim("OrganizationId", organizationId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(2190),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token); // Сериализует объект JwtSecurityToken в строку
        }

    }
}
