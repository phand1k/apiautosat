using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using System.ComponentModel.DataAnnotations;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<IEnumerable<Organization>> GetAllAsync();
        Task AddAsync(Organization organization);
        Task UpdateAsync(Organization organization);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithNumber(string number);
        Task<Organization> GetByNumberAsync(string number);
        Task<Organization> GetByIdAsync(int? id);
        Task<Organization> GetPasswordrganization(double? password);
    }
}
