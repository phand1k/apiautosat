using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.DetailingDTOModels;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Implementations;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using AvtoMigBussines.Detailing.Services.Interfaces;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Detailing.Services.Implementations
{
    public class DetailingServiceService : IDetailingServiceService
    {
        private readonly IDetailingServiceRepository detailingServiceRepository;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IDetailingOrderService detailingOrderService;
        public DetailingServiceService(IDetailingServiceRepository detailingServiceRepository, UserManager<AspNetUser> userManager, IDetailingOrderService detailingOrderService)
        {
            this.detailingServiceRepository = detailingServiceRepository;
            this.userManager = userManager;
            this.detailingOrderService = detailingOrderService;
        }
        public async Task<IEnumerable<DetailingServiceDTO>> GetAllDetailingServicesOnOrder(int? orderId, string? aspNetUserId)
        {
            var detailingServices = await detailingServiceRepository.GetAllDetailingServicesOnOrder(orderId);

            var detailingServiceDTOs = new List<DetailingServiceDTO>();

            foreach (var ws in detailingServices)
            {
                var detailingServiceDTO = new DetailingServiceDTO
                {
                    DetailingServiceId = ws.Id,
                    ServiceName = ws.Service.Name,
                    Price = ws.Price,
                    ServiceId = ws.ServiceId,
                    IsOvered = ws.IsOvered,
                    Salary = ws.Salary,
                    WhomAspNetUserId = ws.AspNetUser.FullName
                    // другие свойства
                };

                detailingServiceDTOs.Add(detailingServiceDTO);
            }

            return detailingServiceDTOs;
        }
        public async Task<double?> GetSummAllServices(int? orderId)
        {
            return await detailingServiceRepository.GetSummAllServices(orderId);
        }
        public async Task<bool> CreateDetailingServiceAsync(DetailingServiceDTO detailingServiceDTO, string aspNetUserId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            var detailingOrder = await detailingOrderService.GetDetailingOrderByIdAsync(Convert.ToInt32(detailingServiceDTO.DetailingOrderId));
            if (detailingOrder == null)
            {
                throw new CustomException.WashOrderNotFoundException("Detailing order not found.");
            }

            var detailingService = new DetailingService()
            {
                DetailingOrderId = detailingServiceDTO.DetailingOrderId,
                ServiceId = detailingServiceDTO.ServiceId,
                Price = detailingServiceDTO.Price,
                OrganizationId = user.OrganizationId,
                AspNetUserId = aspNetUserId,
                WhomAspNetUserId = detailingServiceDTO.WhomAspNetUserId,
                Salary = detailingServiceDTO.Salary,
            };

            await detailingServiceRepository.AddAsync(detailingService);
            return true;
        }

        public async Task DeleteDetailingServiceAsync(int id)
        {
            await detailingServiceRepository.DeleteAsync(id);
        }

        public async Task<DetailingService> GetDetailingServiceByIdAsync(int id)
        {
            return await detailingServiceRepository.GetByIdAsync(id);
        }
    }
}
