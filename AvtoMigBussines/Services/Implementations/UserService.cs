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
using RestSharp;
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
        public async Task<bool> ConfirmCode(double? code, string? phoneNumber)
        {
            return await userRepository.ConfirmForgotPassword(code, phoneNumber);
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
            await userManager.AddToRoleAsync(aspNetUser, "Мастер");
        }

        public async Task<string> ConfirmResetPasswordCodeAndGenerateToken(double? code, string? phoneNumber)
        {
            // Проверяем, существует ли уже пользователь с таким номером телефона
            var user = await userManager.FindByNameAsync(phoneNumber);
            if (user == null || user.IsDeleted == true)
            {
                throw new ArgumentException("Invalid username or user is deleted.");
            }

            // Проверяем корректность кода
            if (!await userRepository.ConfirmForgotPassword(code, phoneNumber))
            {
                throw new Exception("Неправильный код");
            }

            // Получаем роли пользователя
            var userRoles = await userManager.GetRolesAsync(user);

            // Формируем список авторизационных клеймов
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

            // Добавляем роли в клеймы
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            if (user.OrganizationId.HasValue)
            {
                authClaims.Add(new Claim("OrganizationId", user.OrganizationId.Value.ToString()));
            }

            // Генерация JWT токена
            var token = GetToken(authClaims, user.Id, user.OrganizationId);
            return token;
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
        public async Task<bool> ConfirmResetPasswordCode(double? code, string? phoneNumber)
        {
            // Проверяем, существует ли уже пользователь с таким номером телефона
            if (await userRepository.ConfirmForgotPassword(code, phoneNumber) == false)
            {
                throw new Exception("Code wrong");
            }

            return true;
        }
        public async Task ResetPasswordWithWhatsapp(string? phoneNumber)
        {
            var user = await userManager.FindByNameAsync(phoneNumber);
            if (user == null || user.IsDeleted == true)
            {
                throw new ArgumentException("Invalid username or password.");
            }
            try
            {
                var url = "https://api.ultramsg.com/instance95613/messages/chat";
                var client = new RestClient(url);

                var request = new RestRequest(url, Method.Post);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", "pt12bzpf9g1votn1");
                request.AddParameter("to", "%" + phoneNumber);
                Random rnd = new Random();
                double code = rnd.Next(1000, 9999);
                request.AddParameter("body", "Ваш код подтверждения AutoSat: " + code);
                await userRepository.RegisterForgotPasswordCode(code, phoneNumber);

                RestResponse response = await client.ExecuteAsync(request);
                var output = response.Content;

                if (response.IsSuccessful)
                {
                    Console.WriteLine("Whatsapp sms успешно отправлено.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception("Bad request");
                }
                else
                {
                    throw new Exception("Server exception");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error"+ex.Message);
            }
        }
        public async Task ResetPassword(string? phoneNumber)
        {
            var user = await userManager.FindByNameAsync(phoneNumber);
            if (user == null || user.IsDeleted == true)
            {
                throw new ArgumentException("Invalid username or password.");
            }
            using (var httpClient = new HttpClient())
            {
                // Ваш API-ключ
                string apiKey = "kz25d936dcadf4d4b9a77c4c4d3bb8f7864809e554b3c1e930e0ac3ef2e7d2973d1223";
                Random rnd = new Random();
                double code = rnd.Next(1000, 9999);
                // Подготовка URL запроса
                await userRepository.RegisterForgotPasswordCode(code, phoneNumber);
                string requestUrl = $"https://api.mobizon.kz/service/message/sendsmsmessage?recipient={phoneNumber}&from=&text=Код для сброса пароля AutoSat: {code} \nКод действует в течении 20 минут&apiKey={apiKey}";

                try
                {
                    // Отправка GET запроса
                    HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode(); // Выбросит исключение, если статус не 2xx

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Здесь можно обработать успешный ответ, если нужно
                    
                    Console.WriteLine("SMS успешно отправлено."+responseBody);
                }
                catch (HttpRequestException e)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при отправке SMS: {e.Message}");
                    throw;
                }
            }
        }


    }
}
