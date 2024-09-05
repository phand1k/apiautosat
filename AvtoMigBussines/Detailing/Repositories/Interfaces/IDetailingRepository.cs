using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.Detailing.Repositories.Interfaces
{
    public interface IDetailingRepository
    {
        Task<DetailingOrder> GetByIdAsync(int id);
        Task AddAsync(DetailingOrder detailingOrder);
        Task UpdateAsync(DetailingOrder detailingOrder);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(string carNumber, int? organizationId);
        Task<IEnumerable<DetailingOrder>> GetAllFilterAsync(string? aspNetUserId, int? organizationId);

        Task<IEnumerable<DetailingOrder>> GetAllNotCompeltedOrders(string? aspNetUserId, int? organizationId);
        Task CompleteUpdateAsync(DetailingOrder detailingOrder);
        Task<DetailingOrder> GetByIdForCompleteAsync(int id);
        Task<IEnumerable<DetailingOrder>> GettAllCompletedFilterAsync(string? aspNetUserId, int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd);
    }
}
