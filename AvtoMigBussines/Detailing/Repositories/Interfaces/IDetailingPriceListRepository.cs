using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.Detailing.Repositories.Interfaces
{
    public interface IDetailingPriceListRepository
    {
        Task<IEnumerable<DetailingPriceList>> GetAllPriceListForService(int? serviceId, int? carId, int? modelCarId, int? organizationId);
        Task<IEnumerable<DetailingPriceList>> GetAllServiceList(int? serviceId, int? organizationId);
        Task<DetailingPriceList> GetByIdAsync(int id);
        Task AddAsync(DetailingPriceList detailingPriceList);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(int? carId, int? modelCarId, int? serviceId, int? organizationId, double? price);
    }
}
