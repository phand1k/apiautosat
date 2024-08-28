using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Detailing.DetailingDTOModels;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Implementations;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Implementations;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Detailing.Services.Implementations
{
    public class DetailingPriceListService : IDetailingPriceListService
    {
        private readonly IDetailingPriceListRepository detailingPriceListRepository;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IServiceRepository serviceRepository;
        private readonly IDetailingRepository detailingRepository;
        public DetailingPriceListService(IDetailingPriceListRepository detailingPriceListRepository, UserManager<AspNetUser> userManager, IServiceRepository serviceRepository, IDetailingRepository detailingRepository)
        {
            this.detailingPriceListRepository = detailingPriceListRepository;
            this.userManager = userManager;
            this.serviceRepository = serviceRepository;
            this.detailingRepository = detailingRepository;
        }

        public async Task<bool> CreatePriceListAsync(string aspNetUserId, int? serviceId, int? detailingOrderId, double? price)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            var serviceExists = await serviceRepository.GetByIdAsync((int)serviceId);
            if (serviceExists == null)
            {
                throw new CustomException.WashOrderNotFoundException("Service not found.");
            }
            var detailingExists = await detailingRepository.GetByIdAsync((int)detailingOrderId);
            if (detailingExists == null)
            {
                throw new CustomException.WashOrderNotFoundException("Detailing order not found.");
            }

            DetailingPriceList detailingPriceList = new DetailingPriceList
            {
                ServiceId = serviceId,
                AspNetUserId = user.Id,
                OrganizationId = user.OrganizationId,
                CarId = detailingExists.CarId,
                ModelCarId = detailingExists.ModelCarId,
                Price = price
            };

            bool priceListExists = await detailingPriceListRepository
                .ExistsWithName(detailingPriceList.CarId, detailingPriceList.ModelCarId, detailingPriceList.ServiceId, detailingPriceList.OrganizationId, detailingPriceList.Price);

            if (!priceListExists)
            {
                await detailingPriceListRepository.AddAsync(detailingPriceList);
            }

            return true;
        }
        public async Task<IEnumerable<DetailingPriceList>> GetAllServices(int? serviceId, int? carId, int? modelCarId, int? organizationId)
        {
            return await detailingPriceListRepository.GetAllPriceListForService(serviceId, carId, modelCarId, organizationId);
        }
        public Task<IEnumerable<DetailingOrder>> GetAllPriceList(string? aspNetUserId, int? organizationId, int? serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<DetailingPriceList> GetPriceListByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
