using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task<Service> GetByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllAsync(int? organizationId);
        Task AddAsync(Service service);
        Task UpdateAsync(Service service);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(string name, int? organizationId);
    }
}
