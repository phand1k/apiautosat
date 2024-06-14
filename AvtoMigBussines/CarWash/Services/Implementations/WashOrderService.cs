using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.CarWash.Services.Implementations
{
    public class WashOrderService : IWashOrderService
    {
        private readonly IWashOrderRepository _washOrderRepository;
        private readonly UserManager<AspNetUser> userManager;
        public WashOrderService( IWashOrderRepository washOrderRepository, UserManager<AspNetUser> userManager)
        {
            _washOrderRepository = washOrderRepository;
            this.userManager = userManager;
        }
        public async Task<WashOrder> GetWashOrderByIdAsync(int id)
        {
            return await _washOrderRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<WashOrder>> GetAllWashOrdersAsync()
        {
            return await _washOrderRepository.GetAllAsync();
        }
        public async Task<IEnumerable<WashOrder>> GetAllWashOrdersFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await _washOrderRepository.GetAllFilterAsync(aspNetUserId, organizationId);
        }
        public async Task<bool> CreateWashOrderAsync(WashOrder washOrder, string aspNetUserId)
        {
            // Проверяем, существует ли уже автомобиль с таким наименованием
            var user = await userManager.FindByIdAsync(aspNetUserId);
            if (await _washOrderRepository.ExistsWithName(washOrder.CarNumber, user.OrganizationId))
            {
                throw new CustomException.WashOrderExistsException("Wash order with the same car number already exists.");
            }
            var currentUser = await userManager.FindByIdAsync(aspNetUserId);

            washOrder.AspNetUserId = aspNetUserId;
            washOrder.OrganizationId = currentUser.OrganizationId;
            await _washOrderRepository.AddAsync(washOrder);
            return true;
        }

        public async Task UpdateWashOrderAsync(WashOrder washOrder)
        {
            await _washOrderRepository.UpdateAsync(washOrder);
        }

        public async Task DeleteWashOrderAsync(int id)
        {
            await _washOrderRepository.DeleteAsync(id);
        }
    }
}
