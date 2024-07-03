using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Implementations;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.CarWash.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.CarWash.Services.Implementations
{
    public class WashOrderTransactionService : IWashOrderTransactionService
    {
        private readonly IWashOrderTransactionRepository washOrderTransactionRepository;
        private readonly UserManager<AspNetUser> userManager;
        public WashOrderTransactionService(IWashOrderTransactionRepository washTransactionRepository, UserManager<AspNetUser> userManager)
        {
            this.washOrderTransactionRepository = washTransactionRepository;
            this.userManager = userManager;
        }

        public async Task<bool> CreateWashOrderTransactionAsync(WashOrderTransaction washOrderTransaction, string aspNetUserId, int washOrderId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            washOrderTransaction.AspNetUserId = aspNetUserId;
            washOrderTransaction.OrganizationId = user.OrganizationId;
            washOrderTransaction.WashOrderId = washOrderId;
            await washOrderTransactionRepository.AddAsync(washOrderTransaction);
            return true;
        }

        public async Task<IEnumerable<WashOrderTransaction>> GetAllWashOrderTransactionsAsync(int? washOrderId)
        {
            throw new NotImplementedException();
        }

        public async Task<WashOrderTransaction> GetWashOrderTransactionByIdAsync(int id)
        {
            return await washOrderTransactionRepository.GetByIdAsync(id);
        }
    }
}
