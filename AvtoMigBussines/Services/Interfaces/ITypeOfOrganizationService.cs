using AvtoMigBussines.Authenticate;
namespace AvtoMigBussines.Services.Interfaces
{
    public interface ITypeOfOrganizationService
    {
        Task<IEnumerable<TypeOfOrganization>> GetAllTypesAsync();
    }
}
