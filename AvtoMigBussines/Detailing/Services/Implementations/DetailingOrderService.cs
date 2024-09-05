using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Implementations;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace AvtoMigBussines.Detailing.Services.Implementations
{
    public class DetailingOrderService : IDetailingOrderService
    {
        private readonly IDetailingRepository detailingRepository;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IDetailingServiceRepository detailingServiceRepository;
        public DetailingOrderService(IDetailingRepository detailingRepository, UserManager<AspNetUser> userManager, IDetailingServiceRepository detailingServiceRepository)
        {
            this.detailingRepository = detailingRepository;
            this.userManager = userManager;
            this.detailingServiceRepository = detailingServiceRepository;
        }
        public async Task<IEnumerable<DetailingOrder>> GettAllCompletedDetailingOrdersFilterAsync(string? aspNetUserId, int? organizationId, DateTime? dateOfStart, DateTime? dateOfEnd)
        {
            return await detailingRepository.GettAllCompletedFilterAsync(aspNetUserId, organizationId, dateOfStart, dateOfEnd);
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
            detailingOrder.Prepayment = 0;
            await detailingRepository.AddAsync(detailingOrder);
            return true;
        }
        public async Task<bool> CompleteUpdateDetailingOrderAsync(DetailingOrder detailingOrder, string whoIs)
        {
            var notCompletedServices = await detailingServiceRepository.GetAllServicesByDetailingOrderIdAsync(detailingOrder.Id);
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            bool hasUpdated = false;
            detailingOrder.EndOfOrderAspNetUserId = whoIs;
            foreach (var item in notCompletedServices)
            {
                if (item.IsOvered == false)
                {
                    item.IsOvered = true;
                    item.DateOfCompleteService = now.InZone(timeZone).ToDateTimeUnspecified();
                    await detailingServiceRepository.UpdateAsync(item);
                    hasUpdated = true;
                }
            }

            // Если были обновлены услуги или все услуги уже завершены, завершить заказ наряд
            if (hasUpdated || notCompletedServices.All(s => s.IsOvered == true))
            {
                detailingOrder.DateOfCompleteService = now.InZone(timeZone).ToDateTimeUnspecified();
                detailingOrder.IsOvered = true;
                await detailingRepository.CompleteUpdateAsync(detailingOrder);
                return true;
            }

            return false;
        }
        public async Task<DetailingOrder> GetByIdDetailingOrderForComplete(int id)
        {
            return await detailingRepository.GetByIdForCompleteAsync(id);
        }
        public async Task<bool> DeleteUpdateDetailingOrderAsync(DetailingOrder detailingOrder)
        {
            var allDetailingServices = await detailingServiceRepository.GetAllServicesByDetailingOrderIdAsync(detailingOrder.Id);
            foreach (var item in allDetailingServices)
            {
                item.IsDeleted = true;
                await detailingServiceRepository.UpdateAsync(item);
            }
            detailingOrder.IsDeleted = true;
            await detailingRepository.CompleteUpdateAsync(detailingOrder);
            return true;
        }

        public async Task<IEnumerable<DetailingOrder>> GetAllDetailingOrdersFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await detailingRepository.GetAllFilterAsync(aspNetUserId, organizationId);
        }


        public async Task<IEnumerable<DetailingOrder>> GetAllOrdersNotCompletedFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await detailingRepository.GetAllFilterAsync(aspNetUserId, organizationId);
        }


        public async Task<DetailingOrder> GetDetailingOrderByIdAsync(int id)
        {
            return await detailingRepository.GetByIdAsync(id);
        }
    }
}
