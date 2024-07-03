using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface ISalarySettingRepository
    {
        Task<SalarySetting> GetSalaryForUser(string? userId, int? serviceId);
        Task<SalarySetting> GetByIdAsync(int id);
        Task<IEnumerable<SalarySetting>> GetAllAsync();
        Task AddAsync(SalarySetting salarySetting);
        Task UpdateAsync(SalarySetting salarySetting);
        Task DeleteAsync(int id);
        Task<bool> ExistsSalarySetting(int? serviceId, string? aspNetUserId);
    }
}
