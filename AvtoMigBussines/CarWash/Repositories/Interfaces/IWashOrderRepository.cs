using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.CarWash.Repositories.Interfaces
{
    public interface IWashOrderRepository
    {
        Task<WashOrder> GetByIdAsync(int id);
        Task<IEnumerable<WashOrder>> GetAllAsync();
        Task AddAsync(WashOrder carWashOrder);
        Task UpdateAsync(WashOrder carWashOrder);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(string carNumber, int? organizationId);
        Task<IEnumerable<WashOrder>> GetAllFilterAsync(string? aspNetUserId, int? organizationId);
    }
}
