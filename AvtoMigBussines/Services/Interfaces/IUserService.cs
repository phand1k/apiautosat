using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Authenticate.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface IUserService
    {
        Task<AspNetUser> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<AspNetUser> GetUserByIdAsync(string id);
        Task<IEnumerable<AspNetUser>> GetAllUsersAsync(int? organizationId);
        Task<bool> CreateUserAsync(AspNetUser aspNetUser);
        Task UpdateUserAsync(AspNetUser car);
        Task DeleteUserAsync(string id);
        Task <string> LoginUserAsync (LoginModel model);
        Task RegisterUserAsync(RegisterModel model);

        Task ResetPassword(string? phoneNumber);
        Task ResetPasswordWithWhatsapp(string? phoneNumber);
        Task<bool> ConfirmCode(double? code, string? phoneNumber);

        Task<string> ConfirmResetPasswordCodeAndGenerateToken(double? code, string? phoneNumber);
    }
}
