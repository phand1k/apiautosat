using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.Detailing.Services.Interfaces
{
    public interface IDetailingPriceListService
    {
        Task<DetailingPriceList> GetPriceListByIdAsync(int id);
        Task<IEnumerable<DetailingOrder>> GetAllPriceList(string? aspNetUserId, int? organizationId, int?serviceId);
        Task<bool> CreatePriceListAsync(string aspNetUserId, int? serviceId, int? detailingOrderId, double? price);
        Task<IEnumerable<DetailingPriceList>> GetAllServices(int? serviceId, int? carId, int? modelCarId, int? organizationId);

    }
}
