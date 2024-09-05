using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.CarWash.Services.Interfaces
{
    public interface IWashOrderTransactionService
    {
        Task<WashOrderTransaction> GetWashOrderTransactionByIdAsync(int id);
        Task<bool> CreateWashOrderTransactionAsync(WashOrderTransaction washOrderTransaction, string aspNetUserId, int washOrderId);
        Task<IEnumerable<WashOrderTransaction>> GetAllTransactions(string? aspNetUserId, DateTime? dateOfStart, DateTime? dateOfEnd);
        Task<IEnumerable<DetailingOrderTransaction>> GetAllDetailingOrderTransactions(string? aspNetUserId, DateTime? dateOfStart, DateTime? dateOfEnd);
        Task<bool> CreateDetailingOrderTransactionAsync(DetailingOrderTransaction detailingOrderTransaction, string? aspNetUserId, int detailOrderId);
    }
}
