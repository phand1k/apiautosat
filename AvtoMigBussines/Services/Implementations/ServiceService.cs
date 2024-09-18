using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Exceptions;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace AvtoMigBussines.Services.Implementations
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository serviceRepository;
        private readonly UserManager<AspNetUser> userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        public ServiceService(IServiceRepository serviceRepository, UserManager<AspNetUser> userManager, IHttpClientFactory httpClientFactory)
        {
            this.serviceRepository = serviceRepository;
            this.userManager = userManager;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateServiceAsync(Service service, string? aspNetUserId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            if (user == null)
            {
                throw new CustomException.UserNotFoundException("User not found");
            }
            if (await serviceRepository.ExistsWithName(service.Name, user.OrganizationId))
            {
                throw new CustomException.ServiceExistsException("Service with the same name already exists.");
            }
            service.OrganizationId = user.OrganizationId;
            service.AspNetUserId = user.Id;
            await serviceRepository.AddAsync(service);
            return true;
        }

        public async Task DeleteServiceAsync(int id)
        {
            await serviceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync(string? aspNetUserId)
        {
            var user = await userManager.FindByIdAsync(aspNetUserId);
            if (user == null)
            {
                throw new CustomException.UserNotFoundException("User not found");
            }
            return await serviceRepository.GetAllAsync(user.OrganizationId);
        }

        public async Task<Service> GetServiceByIdAsync(int id)
        {
            return await serviceRepository.GetByIdAsync(id);
        }

        public async Task UpdateServiceAsync(Service car)
        {
            await serviceRepository.UpdateAsync(car);
        }
        public async Task ChangePriceServiceAsync(int serviceId, double newPrice)
        {
            Service service = await serviceRepository.GetByIdAsync(serviceId);
            service.Price = newPrice;
            await serviceRepository.UpdateAsync(service);
        }
    }
}
