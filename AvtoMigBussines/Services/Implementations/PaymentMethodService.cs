using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Services.Implementations
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly UserManager<AspNetUser> userManager;
        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, UserManager<AspNetUser> userManager)
        {
            this.paymentMethodRepository = paymentMethodRepository;
            this.userManager = userManager;
        }
        public async Task<bool> CreateAsync(PaymentMethod paymentMethod)
        {
            await paymentMethodRepository.AddAsync(paymentMethod);
            return true;
        }
        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync()
        {
            return await paymentMethodRepository.GetAllAsync();
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
