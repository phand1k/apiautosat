using AvtoMigBussines.CarWash.Models;

namespace AvtoMigBussines.CarWash.Repositories.Interfaces
{
    public interface IWashOrderTransactionRepository
    {
        Task<WashOrderTransaction> GetByIdAsync(int id);
        Task<IEnumerable<WashOrderTransaction>> GetAllAsync(int? washOrderId);
        Task AddAsync(WashOrderTransaction washOrderTransaction);
        Task UpdateAsync(WashOrderTransaction washOrderTransaction);
        Task DeleteAsync(int id);
    }
}
