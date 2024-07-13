using AvtoMigBussines.CarWash.Models;

namespace AvtoMigBussines.CarWash.Repositories.Interfaces
{
    public interface IWashOrderTransactionRepository
    {
        Task<WashOrderTransaction> GetByIdAsync(int id);
        Task<IEnumerable<WashOrderTransaction>> GetAllAsync(int? organizationId);
        Task AddAsync(WashOrderTransaction washOrderTransaction);
        Task UpdateAsync(WashOrderTransaction washOrderTransaction);
        Task DeleteAsync(int id);
        Task<IEnumerable<WashOrderTransaction>> GetAllTransactions(int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd);
    }
}
