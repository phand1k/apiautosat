using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.Detailing.Repositories.Interfaces
{
    public interface IDetailingServiceRepository
    {
        Task<DetailingService> GetByIdAsync(int id);
        Task AddAsync(DetailingService detailingService);
        Task DeleteAsync(int id);
        Task<IEnumerable<DetailingService>> GetAllFilterAsync(string? aspNetUserId, int? organizationId);
        Task<double?> GetSummAllServices(int? orderId);
        Task<IEnumerable<DetailingService>> GetAllServicesByDetailingOrderIdAsync(int id);
        Task UpdateAsync(DetailingService detailingService);
        Task<IEnumerable<DetailingService>> GetAllDetailingServicesOnOrder(int? orderId);
    }
}
