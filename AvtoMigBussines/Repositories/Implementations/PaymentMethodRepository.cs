using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly ApplicationDbContext context;
        public PaymentMethodRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(PaymentMethod paymentMethod)
        {
            context.PaymentMethods.Add(paymentMethod);
            await context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            return await context.PaymentMethods.Where(x => x.IsDeleted == false).ToListAsync();
        }

        public Task<PaymentMethod> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task UpdateAsync(PaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }
    }
}
