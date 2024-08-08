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
        private readonly IServiceRepository serviceRepository;

        public SalarySettingService(IServiceRepository serviceRepository, ISalarySettingRepository salarySettingRepository, UserManager<AspNetUser> userManager)
        {
            this.salarySettingRepository = salarySettingRepository;
            this.userManager = userManager;
            this.serviceRepository = serviceRepository;
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
                AspNetUserId = salarySettingDTO.AspNetUserId,
                OrganizationId = user.Result.OrganizationId
            };
            if (salarySettingDTO.Salary <= 100)
            {
                salarySetting.Salary = (serviceRepository.GetByIdAsync(salarySettingDTO.ServiceId).Result.Price*salarySettingDTO.Salary)/100;
            }
            else
            {
                salarySetting.Salary = salarySettingDTO.Salary;
            }

            await salarySettingRepository.AddAsync(salarySetting);
            return true;
        }

        public Task DeleteSalarySettingAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SalarySetting>> GetAllSalarySettingsAsync(int? organizationId)
        {
            return await salarySettingRepository.GetAllAsync(organizationId);
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
