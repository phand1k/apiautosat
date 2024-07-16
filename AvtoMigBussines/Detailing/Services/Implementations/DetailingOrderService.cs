using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Implementations;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Detailing.Services.Implementations
{
    public class DetailingOrderService : IDetailingOrderService
    {
        private readonly IDetailingRepository detailingRepository;
        private readonly UserManager<AspNetUser> userManager;
        public DetailingOrderService(IDetailingRepository detailingRepository, UserManager<AspNetUser> userManager)
        {
            this.detailingRepository = detailingRepository;
            this.userManager = userManager;
        }
        public async Task<bool> CreateDetailingOrderAsync(DetailingOrder detailingOrder, string aspNetUserId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            if (await detailingRepository.ExistsWithName(detailingOrder.CarNumber, user.OrganizationId))
            {
                throw new CustomException.WashOrderExistsException("Detailing order with the same car number already exists.");
            }

            var currentUser = await userManager.FindByIdAsync(aspNetUserId);

            detailingOrder.AspNetUserId = aspNetUserId;
            detailingOrder.OrganizationId = currentUser.OrganizationId;
            await detailingRepository.AddAsync(detailingOrder);
            return true;
        }

        public async Task DeleteDetailingOrderAsync(int id)
        {
            await detailingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DetailingOrder>> GetAllDetailingOrdersFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await detailingRepository.GetAllFilterAsync(aspNetUserId, organizationId);
        }

        public async Task<DetailingOrder> GetDetailingOrderByIdAsync(int id)
        {
            return await detailingRepository.GetByIdAsync(id);
        }
    }
}
