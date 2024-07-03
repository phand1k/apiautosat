using AvtoMigBussines.CarWash.Models;

namespace AvtoMigBussines.CarWash.Services.Interfaces
{
    public interface IWashOrderTransactionService
    {
        Task<WashOrderTransaction> GetWashOrderTransactionByIdAsync(int id);
        Task<IEnumerable<WashOrderTransaction>> GetAllWashOrderTransactionsAsync(int? washOrderId);
        Task<bool> CreateWashOrderTransactionAsync(WashOrderTransaction washOrderTransaction, string aspNetUserId, int washOrderId);
    }
}
