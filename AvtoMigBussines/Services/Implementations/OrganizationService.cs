using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Services.Implementations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository organizationRepository;
        private readonly UserManager<AspNetUser> userManager;
        public OrganizationService(IOrganizationRepository organizationRepository, UserManager<AspNetUser> userManager)
        {
            this.organizationRepository = organizationRepository;
            this.userManager = userManager;
        }
        public async Task<Organization> GetOrganizationByIdAsync(int? id)
        {
            return await organizationRepository.GetByIdAsync(id);
        }
        public async Task<Organization> GetOrganizationByNumberAsync(string number)
        {
            return await organizationRepository.GetByNumberAsync(number);
        }
        public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
        {
            return await organizationRepository.GetAllAsync();
        }

        public async Task<bool> CreateOrganizationAsync(Organization organization)
        {
            // Проверяем, существует ли уже автомобиль с таким наименованием
            if (await organizationRepository.ExistsWithNumber(organization.Number))
            {
                throw new CustomException.OrganizationExistsException("Organization with the same number already exists.");
            }

            await organizationRepository.AddAsync(organization);
            Random rnd = new Random();
            AspNetUser defaultUser = new AspNetUser
            {
                FirstName = "Стандартный",
                LastName = "Пользователь",
                SurName = "Для назначения услуг",
                OrganizationId = organization.Id,
                PhoneNumber = Guid.NewGuid().ToString()
            };
            await userManager.CreateAsync(defaultUser);
            return true;
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            await organizationRepository.UpdateAsync(organization);
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            await organizationRepository.DeleteAsync(id);
        }
    }
}
