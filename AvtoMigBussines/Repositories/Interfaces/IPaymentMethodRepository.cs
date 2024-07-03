using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod> GetByIdAsync(int id);
        Task<IEnumerable<PaymentMethod>> GetAllAsync();
        Task AddAsync(PaymentMethod paymentMethod);
        Task UpdateAsync(PaymentMethod paymentMethod);
        Task DeleteAsync(int id);
    }
}
