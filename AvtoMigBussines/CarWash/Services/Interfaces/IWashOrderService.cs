using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.CarWash.Services.Interfaces
{
    public interface IWashOrderService
    {
        Task<WashOrder> GetWashOrderByIdAsync(int id);
        Task<IEnumerable<WashOrder>> GetAllWashOrdersAsync();
        Task<IEnumerable<WashOrder>> GetAllWashOrdersFilterAsync(string? aspNetUserId, int? organizationId);
        Task<IEnumerable<WashOrder>> GetAllWashOrdersNotCompletedFilterAsync(string? aspNetUserId, int? organizationId);
        Task<IEnumerable<WashOrder>> GettAllCompletedWashOrdersFilterAsync(string? aspNetUserId, int? organizationId);
        Task<bool> CreateWashOrderAsync(WashOrder washOrder, string aspNetUserId);
        Task UpdateWashOrderAsync(WashOrder washOrder);
        Task DeleteWashOrderAsync(int id);
        Task <bool> CompleteUpdateWashOrderAsync(WashOrder washOrder, string? whoIsEnd);
        Task<bool> DeleteUpdateWashOrderAsync(WashOrder washOrder);
        Task<int?> GetCountOfNotCompletedWashOrdersAsync(string? aspNetUserId, int? organizationId);
    }
}
