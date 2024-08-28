using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.CarWash.Repositories.Interfaces
{
    public interface IWashOrderRepository
    {
        Task<WashOrder> GetByIdForCompleteAsync(int id);
        Task<WashOrder> GetByIdAsync(int id);
        Task<IEnumerable<WashOrder>> GetAllAsync();
        Task AddAsync(WashOrder carWashOrder);
        Task UpdateAsync(WashOrder carWashOrder);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(string carNumber, int? organizationId);
        Task<IEnumerable<WashOrder>> GetAllFilterAsync(string? aspNetUserId, int? organizationId);
        Task CompleteUpdateAsync(WashOrder carWashOrder);
        Task<IEnumerable<WashOrder>> GetAllNotCompletedFilterAsync(string? aspNetUserId, int? organizationId);
        Task<IEnumerable<WashOrder>> GettAllCompletedFilterAsync(string? aspNetUserId, int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd);
        Task<int?> GetAllNotCompletedCountFilterAsync(string? aspNetUserId, int? organizationId);
        Task ReturnAsync(int id);
    }
}
