using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;

namespace AvtoMigBussines.CarWash.Repositories.Interfaces
{
    public interface IWashServiceRepository
    {
        Task<WashService> GetByIdAsync(int id);
        Task<IEnumerable<WashService>> GetAllAsync();
        Task AddAsync(WashService washService);
        Task UpdateAsync(WashService washService);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(int? orderId, int? serviceId);
        Task<IEnumerable<WashService>> GetAllFilterAsync(string? aspNetUserId, int? organizationId);
        Task<int?> GetCountAllServices(int? orderId);
        Task<double?> GetSummAllServices(int? orderId);
        Task<IEnumerable<WashService>> GetAllWashServicesOnOrder(int? orderId);
    }
}
