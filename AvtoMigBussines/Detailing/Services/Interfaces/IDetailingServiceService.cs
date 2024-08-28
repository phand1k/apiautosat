using AvtoMigBussines.Detailing.DetailingDTOModels;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.DTOModels;

namespace AvtoMigBussines.Detailing.Services.Interfaces
{
    public interface IDetailingServiceService
    {
        Task<DetailingService> GetDetailingServiceByIdAsync(int id);
        Task<bool> CreateDetailingServiceAsync(DetailingServiceDTO detailingServiceDTO, string aspNetUserId);
        Task DeleteDetailingServiceAsync(int id);
        Task<double?> GetSummAllServices(int? orderId);
        Task<IEnumerable<DetailingServiceDTO>> GetAllDetailingServicesOnOrder(int? orderId, string? aspNetUserId);
    }
}
