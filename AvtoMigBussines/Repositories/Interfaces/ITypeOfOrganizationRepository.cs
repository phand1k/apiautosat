using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface ITypeOfOrganizationRepository
    {
        Task<IEnumerable<TypeOfOrganization>> GetAllAsync();
        Task<string> GetByIdAsync(int? id);
    }
}
