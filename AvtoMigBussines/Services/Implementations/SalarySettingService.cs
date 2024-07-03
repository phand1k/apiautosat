using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Services.Implementations
{
    public class SalarySettingService : ISalarySettingService
    {
        private readonly ISalarySettingRepository salarySettingRepository;
        private readonly UserManager<AspNetUser> userManager;

        public SalarySettingService(ISalarySettingRepository salarySettingRepository, UserManager<AspNetUser> userManager)
        {
            this.salarySettingRepository = salarySettingRepository;
            this.userManager = userManager;
        }

        public async Task<bool> CreateSalarySettingAsync(SalarySettingDTO salarySettingDTO)
        {
            // Проверяем, существует ли уже автомобиль с таким наименованием
            if (await salarySettingRepository.ExistsSalarySetting(salarySettingDTO.ServiceId, salarySettingDTO.AspNetUserId))
            {
                throw new Exception("Salary alreday created for this user.");
            }
            var user = userManager.FindByIdAsync(salarySettingDTO.AspNetUserId);
            var salarySetting = new SalarySetting()
            {
                ServiceId = salarySettingDTO.ServiceId,
                Salary = salarySettingDTO.Salary,
                AspNetUserId = salarySettingDTO.AspNetUserId,
                OrganizationId = user.Result.OrganizationId
            };
            await salarySettingRepository.AddAsync(salarySetting);
            return true;
        }

        public Task DeleteSalarySettingAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SalarySetting>> GetAllSalarySettingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SalarySetting> GetSalarySettingByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<SalarySetting> GetSalarySettingForUser(string? userId, int? serviceId)
        {
            return await salarySettingRepository.GetSalaryForUser(userId, serviceId);
        }

        public Task UpdateSalarySettingAsync(SalarySetting salarySetting)
        {
            throw new NotImplementedException();
        }
    }
}
