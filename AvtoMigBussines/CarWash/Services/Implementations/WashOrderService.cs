using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using Microsoft.AspNetCore.Identity;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvtoMigBussines.CarWash.Services.Implementations
{
    public class WashOrderService : IWashOrderService
    {
        private readonly IWashOrderRepository _washOrderRepository;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IWashServiceRepository washServiceRepository;

        public WashOrderService(IWashOrderRepository washOrderRepository, UserManager<AspNetUser> userManager, IWashServiceRepository washServiceRepository)
        {
            _washOrderRepository = washOrderRepository;
            this.userManager = userManager;
            this.washServiceRepository = washServiceRepository;
        }
        public async Task<WashOrder> GetByIdWashOrderForComplete(int id)
        {
            return await _washOrderRepository.GetByIdForCompleteAsync(id);
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
        public async Task ReturnWashOrderAsync(int id)
        {
            await _washOrderRepository.ReturnAsync(id);
        }
        public async Task<bool> DeleteUpdateWashOrderAsync(WashOrder washOrder)
        {
            var allWashServices = await washServiceRepository.GetAllServicesByWashOrderIdAsync(washOrder.Id);
            foreach (var item in allWashServices)
            {
                item.IsDeleted = true;
                await washServiceRepository.UpdateAsync(item);
            }
            washOrder.IsDeleted = true;
            await _washOrderRepository.CompleteUpdateAsync(washOrder);
            return true;
        }
        public async Task<bool> ReadyUpdateWashOrderAsync(WashOrder washOrder)
        {
            washOrder.IsReady = true;
            await _washOrderRepository.UpdateAsync(washOrder);
            return true;
        }
        public async Task<bool> CompleteUpdateWashOrderAsync(WashOrder washOrder, string whoIs)
        {
            var notCompletedServices = await washServiceRepository.GetAllServicesByWashOrderIdAsync(washOrder.Id);
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            bool hasUpdated = false;
            washOrder.EndOfOrderAspNetUserId = whoIs;
            foreach (var item in notCompletedServices)
            {
                if (item.IsOvered == false)
                {
                    item.IsOvered = true;
                    item.DateOfCompleteService = now.InZone(timeZone).ToDateTimeUnspecified();
                    await washServiceRepository.UpdateAsync(item);
                    hasUpdated = true;
                }
            }

            // Если были обновлены услуги или все услуги уже завершены, завершить заказ наряд
            if (hasUpdated || notCompletedServices.All(s => s.IsOvered == true))
            {
                washOrder.DateOfCompleteService = now.InZone(timeZone).ToDateTimeUnspecified();
                washOrder.IsOvered = true;
                await _washOrderRepository.CompleteUpdateAsync(washOrder);
                return true;
            }

            return false;
        }


        public async Task<IEnumerable<WashOrder>> GetAllWashOrdersNotCompletedFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await _washOrderRepository.GetAllNotCompletedFilterAsync(aspNetUserId, organizationId);
        }

        public async Task<IEnumerable<WashOrder>> GettAllCompletedWashOrdersFilterAsync(string? aspNetUserId, int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd)
        {
            return await _washOrderRepository.GettAllCompletedFilterAsync(aspNetUserId, organizationId, dateOfStart, dateOfEnd);
        }

        public async Task<int?> GetCountOfNotCompletedWashOrdersAsync(string? aspNetUserId, int? organizationId)
        {
            return await _washOrderRepository.GetAllNotCompletedCountFilterAsync(aspNetUserId, organizationId);
        }
    }
}
