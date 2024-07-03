using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.Data;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.CarWash.Repositories.Implementations
{
    public class WashOrderTransactionRepository : IWashOrderTransactionRepository
    {
        private readonly ApplicationDbContext context;
        public WashOrderTransactionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(WashOrderTransaction washOrderTransaction)
        {
            context.WashOrderTransactions.Add(washOrderTransaction);
            await context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WashOrderTransaction>> GetAllAsync(int? washOrderId)
        {
            throw new NotImplementedException();
        }

        public async Task<WashOrderTransaction> GetByIdAsync(int id)
        {
            return await context.WashOrderTransactions.Include(x=>x.PaymentMethod).FirstOrDefaultAsync(x => x.WashOrderId == id);
        }

        public Task UpdateAsync(WashOrderTransaction washOrderTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
