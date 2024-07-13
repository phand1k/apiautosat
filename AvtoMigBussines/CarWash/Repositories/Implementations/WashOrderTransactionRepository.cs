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

        public async Task<IEnumerable<WashOrderTransaction>> GetAllAsync(int? organizationId)
        {
            return await context.WashOrderTransactions.Where(x=>x.OrganizationId == organizationId).Include(x=>x.PaymentMethod).ToListAsync();
        }
        public async Task<IEnumerable<WashOrderTransaction>> GetAllTransactions(int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd)
        {
            // Начальная загрузка данных с необходимыми связями
            var query = context.WashOrderTransactions
                .Include(x => x.PaymentMethod)
                .Include(x => x.Organization)
                .Where(x => x.OrganizationId == organizationId && x.IsDeleted == false);

            // Проверка наличия дат и фильтрация на их основе
            if (dateOfStart != null && dateOfEnd != null)
            {
                query = query.Where(x => x.DateOfCreated >= dateOfStart && x.DateOfCreated <= dateOfEnd);
            }

            return await query.ToListAsync();
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
