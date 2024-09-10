using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.CarWash.Repositories.Interfaces
{
    public interface IWashOrderTransactionRepository
    {
        Task<WashOrderTransaction> GetByIdAsync(int id);
        Task<DetailingOrderTransaction> GetDetailingOrderTransactionById(int id);
        Task<IEnumerable<WashOrderTransaction>> GetAllAsync(int? organizationId);
        Task AddAsync(WashOrderTransaction washOrderTransaction);
        Task AddDetailingOrderTransactionAsync(DetailingOrderTransaction detailingOrderTransaction);
        Task UpdateAsync(WashOrderTransaction washOrderTransaction);
        Task DeleteAsync(int id);
        Task<IEnumerable<WashOrderTransaction>> GetAllTransactions(int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd);
        Task<IEnumerable<DetailingOrderTransaction>> GetAllDetailingOrderTransactions(int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd);
    }
}
