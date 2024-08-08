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
        private readonly IWashServiceService _washService;
        public WashOrderTransactionService(IWashOrderTransactionRepository washTransactionRepository, UserManager<AspNetUser> userManager, IWashServiceService washServiceService)
        {
            this.washOrderTransactionRepository = washTransactionRepository;
            this.userManager = userManager;
            _washService = washServiceService;
        }
        public async Task<IEnumerable<WashOrderTransaction>> GetAllTransactions(string? aspNetUserId, DateTime? dateOfStart, DateTime? dateOfEnd)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            return await washOrderTransactionRepository.GetAllTransactions(user.OrganizationId, dateOfStart, dateOfEnd);
        }
        public async Task<bool> CreateWashOrderTransactionAsync(WashOrderTransaction washOrderTransaction, string aspNetUserId, int washOrderId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            washOrderTransaction.AspNetUserId = aspNetUserId;
            washOrderTransaction.OrganizationId = user.OrganizationId;
            washOrderTransaction.WashOrderId = washOrderId;
            washOrderTransaction.ToPay = await _washService.GetSummAllServices(washOrderId);
            await washOrderTransactionRepository.AddAsync(washOrderTransaction);
            return true;
        }

        public async Task<WashOrderTransaction> GetWashOrderTransactionByIdAsync(int id)
        {
            return await washOrderTransactionRepository.GetByIdAsync(id);
        }
    }
}
