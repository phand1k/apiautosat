using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;

namespace AvtoMigBussines.Services.Implementations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository organizationRepository;
        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            this.organizationRepository = organizationRepository;
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
