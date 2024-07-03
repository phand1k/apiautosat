using AvtoMigBussines.CarWash.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<IEnumerable<WashService>> GetAllMyWashServices(string? aspNetUserId);
        Task<IEnumerable<WashService>> GetAllMyNotCompletedWashServices(string? aspNetUserId);
        Task<IEnumerable<WashService>> GetAllNotCompletedWashServicesOnOrder(int? orderId);
        Task<IEnumerable<WashService>> GetAllServicesByWashOrderIdAsync(int id);
    }
}
