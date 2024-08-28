using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.Detailing.Services.Interfaces
{
    public interface IDetailingOrderService
    {
        Task<DetailingOrder> GetDetailingOrderByIdAsync(int id);
        Task<IEnumerable<DetailingOrder>> GetAllDetailingOrdersFilterAsync(string? aspNetUserId, int? organizationId);
        Task<IEnumerable<DetailingOrder>> GetAllOrdersNotCompletedFilterAsync(string? aspNetUserId, int? organizationId);
        Task<bool> CreateDetailingOrderAsync(DetailingOrder detailingOrder, string aspNetUserId);
        Task<bool> DeleteUpdateDetailingOrderAsync(DetailingOrder detailingOrder);

    }
}
