using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Authenticate.Models;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<AspNetUser> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<AspNetUser>> GetAllAsync(int? organizationId);
        Task AddAsync(AspNetUser aspNetUser);
        Task UpdateAsync(AspNetUser aspNetUser);
        Task DeleteAsync(string id);
        Task<bool> ExistsWithPhoneNumber(string phoneNumber);
        Task RegisterAsync(AspNetUser aspNetUser);
        Task<AspNetUser> GetByIdAsync(string id);
        Task RegisterForgotPasswordCode(double code, string? phoneNumber);

        Task<bool> ConfirmForgotPassword(double? code, string? phoneNumber);
    }
}
