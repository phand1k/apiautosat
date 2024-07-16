using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;

namespace AvtoMigBussines.Services.Implementations
{
    public class TypeOfOrganizationService : ITypeOfOrganizationService
    {
        private readonly ITypeOfOrganizationRepository typeOfOrganizationRepository;
        public TypeOfOrganizationService(ITypeOfOrganizationRepository typeOfOrganizationRepository)
        {
            this.typeOfOrganizationRepository = typeOfOrganizationRepository;
        }

        public async Task<IEnumerable<TypeOfOrganization>> GetAllTypesAsync()
        {
            return await typeOfOrganizationRepository.GetAllAsync();
        }
    }
}
