using AvtoMigBussines.Authenticate;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface ITypeOfOrganizationRepository
    {
        Task<IEnumerable<TypeOfOrganization>> GetAllAsync();
    }
}
