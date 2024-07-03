using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Implementations;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.CarWash.Services.Interfaces;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace AvtoMigBussines.CarWash.Services.Implementations
{
    public class WashServiceService : IWashServiceService
    {
        private readonly IWashServiceRepository _washServiceRepository;
        private readonly UserManager<AspNetUser> userManager;
        public WashServiceService(IWashServiceRepository washServiceRepository, UserManager<AspNetUser> userManager)
        {
            _washServiceRepository = washServiceRepository;
            this.userManager = userManager;
        }
        public async Task<bool> CreateAsync(WashServiceDTO washServiceDTO, string aspNetUserId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            if (await _washServiceRepository.ExistsWithName(washServiceDTO.WashOrderId, washServiceDTO.ServiceId))
            {
                throw new CustomException.WashOrderExistsException("Wash service with the same service already exists.");
            }

            var currentUser = await userManager.FindByIdAsync(aspNetUserId);
            var washService = new WashService()
            {
                WashOrderId = washServiceDTO.WashOrderId,
                ServiceId = washServiceDTO.ServiceId,
                Price = washServiceDTO.Price,
                OrganizationId = user.OrganizationId,
                AspNetUserId = aspNetUserId,
                WhomAspNetUserId = washServiceDTO.WhomAspNetUserId,
                Salary = washServiceDTO.Salary
            };

            await _washServiceRepository.AddAsync(washService);
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            await _washServiceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<WashService>> GetAllAsync()
        {
            return await _washServiceRepository.GetAllAsync();
        }
        public async Task<IEnumerable<WashServiceDTO>> GetAllMyIsNotCompletedWashServices(string? aspNetUserId)
        {
            var allMyWashServices = await _washServiceRepository.GetAllMyNotCompletedWashServices(aspNetUserId);

            var allMyWashServiceDTOs = new List<WashServiceDTO>();

            foreach (var ws in allMyWashServices)
            {
                var washServiceDTO = new WashServiceDTO
                {
                    ServiceName = ws.Service.Name,
                    Price = ws.Price,
                    WhomAspNetUserId = ws.WhomAspNetUserId,
                    Order = ws.WashOrder.ModelCar.Name + " " + ws.WashOrder.Car.Name,
                    CarNumber = ws.WashOrder.CarNumber,
                    WashOrderId = ws.WashOrder.Id,
                    AspNetUserId = ws.AspNetUserId,
                    DateOfCreated = ws.DateOfCreated,
                    Salary = ws.Salary,
                    WashServiceId = ws.Id,
                    DateOfCompleted = ws.DateOfCompleteService
                };

                allMyWashServiceDTOs.Add(washServiceDTO);
            }

            return allMyWashServiceDTOs;
        }
        public async Task<IEnumerable<WashServiceDTO>> GetAllMyWashServices(string? aspNetUserId)
        {
            var allMyWashServices = await _washServiceRepository.GetAllMyWashServices(aspNetUserId);

            var allMyWashServiceDTOs = new List<WashServiceDTO>();

            foreach (var ws in allMyWashServices)
            {
                var washServiceDTO = new WashServiceDTO
                {
                    ServiceName = ws.Service.Name,
                    Price = ws.Price,
                    WhomAspNetUserId = ws.WhomAspNetUserId,
                    Order = ws.WashOrder.ModelCar.Name + " "+ws.WashOrder.Car.Name,
                    CarNumber = ws.WashOrder.CarNumber,
                    WashOrderId = ws.WashOrder.Id,
                    AspNetUserId = ws.AspNetUserId,
                    DateOfCreated = ws.DateOfCreated,
                    Salary = ws.Salary,
                    DateOfCompleted = ws.DateOfCompleteService
                };

                allMyWashServiceDTOs.Add(washServiceDTO);
            }

            return allMyWashServiceDTOs;
        }
        public async Task<IEnumerable<WashServiceDTO>> GetAllNotCompletedWashServicesOnOrder(int? orderId, string? aspNetUserId)
        {
            var washServices = await _washServiceRepository.GetAllNotCompletedWashServicesOnOrder(orderId);

            var washServiceDTOs = new List<WashServiceDTO>();

            foreach (var ws in washServices)
            {
                var washServiceDTO = new WashServiceDTO
                {
                    WashServiceId = ws.Id,
                    ServiceName = ws.Service.Name,
                    Price = ws.Price,
                    ServiceId = ws.ServiceId
                    // другие свойства
                };
                washServiceDTOs.Add(washServiceDTO);
            }

            return washServiceDTOs;
        }
        public async Task<IEnumerable<WashServiceDTO>> GetAllWashServicesOnOrder(int? orderId, string? aspNetUserId)
        {
            var washServices = await _washServiceRepository.GetAllWashServicesOnOrder(orderId);

            var washServiceDTOs = new List<WashServiceDTO>();

            foreach (var ws in washServices)
            {
                var washServiceDTO = new WashServiceDTO
                {
                    WashServiceId = ws.Id,
                    ServiceName = ws.Service.Name,
                    Price = ws.Price,
                    ServiceId = ws.ServiceId,
                    IsOvered = ws.IsOvered,
                    Salary = ws.Salary,
                    WhomAspNetUserId = ws.AspNetUser.FullName
                    // другие свойства
                };

                washServiceDTOs.Add(washServiceDTO);
            }

            return washServiceDTOs;
        }



        public async Task<WashService> GetByIdAsync(int id)
        {
            return await _washServiceRepository.GetByIdAsync(id);
        }

        public async Task<int?> GetCountAllServices(int? orderId)
        {
            return await _washServiceRepository.GetCountAllServices(orderId);
        }

        public async Task<double?> GetSummAllServices(int? orderId)
        {
            return await _washServiceRepository.GetSummAllServices(orderId);
        }

        public async Task UpdateAsync(WashService washService)
        {
            await _washServiceRepository.UpdateAsync(washService);
        }
        public async Task CompleteUpdateAsync(WashService washService)
        {
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Almaty"];
            var now = SystemClock.Instance.GetCurrentInstant();
            washService.DateOfCompleteService = now.InZone(timeZone).ToDateTimeUnspecified();
            washService.IsOvered = true;
            await _washServiceRepository.UpdateAsync(washService);
        }
    }
}
