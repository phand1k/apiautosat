using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<PaymentMethod> GetByIdAsync(int id);
        Task<IEnumerable<PaymentMethod>> GetAllAsync();
        Task<bool> CreateAsync(PaymentMethod paymentMethod);
        Task UpdateAsync(PaymentMethod paymentMethod);
        Task DeleteAsync(int id);
    }
}
