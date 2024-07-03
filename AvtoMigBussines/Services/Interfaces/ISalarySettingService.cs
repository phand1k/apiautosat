using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface ISalarySettingService
    {
        Task<SalarySetting> GetSalarySettingForUser(string? userId, int? serviceId);
        Task<SalarySetting> GetSalarySettingByIdAsync(int id);
        Task<IEnumerable<SalarySetting>> GetAllSalarySettingsAsync();
        Task<bool> CreateSalarySettingAsync(SalarySettingDTO salarySettingDTO);
        Task UpdateSalarySettingAsync(SalarySetting salarySetting);
        Task DeleteSalarySettingAsync(int id);
    }
}
