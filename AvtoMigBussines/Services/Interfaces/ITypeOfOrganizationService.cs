using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface ITypeOfOrganizationService
    {
        Task<IEnumerable<TypeOfOrganization>> GetAllTypesAsync();
        Task<string> GetTypeOrganizationIdAsync(int? id);
    }
}
