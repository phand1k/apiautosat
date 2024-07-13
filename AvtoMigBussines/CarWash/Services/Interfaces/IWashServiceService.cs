using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.CarWash.Services.Interfaces
{
    public interface IWashServiceService
    {
        Task<WashService> GetByIdAsync(int id);
        Task<IEnumerable<WashService>> GetAllAsync();
        Task<bool> CreateAsync(WashServiceDTO washServiceDTO, string aspNetUserId);
        Task UpdateAsync(WashService washService);
        Task CompleteUpdateAsync(WashService washService);
        Task DeleteAsync(int id);
        Task<int?> GetCountAllServices(int? orderId);
        Task<double?> GetSummAllServices(int? orderId);
        Task<IEnumerable<WashServiceDTO>> GetAllWashServicesOnOrder(int? orderId, string? aspNetUserId);
        Task<IEnumerable<WashServiceDTO>> GetAllMyWashServices(string? aspNetUserId);
        Task<IEnumerable<WashServiceDTO>> GetAllMyIsNotCompletedWashServices(string? aspNetUserId);
        Task<IEnumerable<WashServiceDTO>> GetAllNotCompletedWashServicesOnOrder(int? orderId, string? aspNetUserId);
        Task<int?> GetCountOfNotCompletedServicesOnNotCompletedOrders(int? organizationId);
        Task<int?> GetCountOfCompletedServicesOnNotCompletedOrders(int? organizationId);
        Task<double?> GetSummOfServicesOnNotCompletedWashOrders(int? organizationId);
        Task<IEnumerable<WashService>> GetAllWashServicesWithPhoneNumber(string? phoneNumber);
    }
}
