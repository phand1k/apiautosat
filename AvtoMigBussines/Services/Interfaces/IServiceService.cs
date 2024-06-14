using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface IServiceService
    {
        Task<Service> GetServiceByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllServicesAsync(string? aspNetUserId);
        Task<bool> CreateServiceAsync(Service car, string? aspNetUserId);
        Task UpdateServiceAsync(Service car);
        Task DeleteServiceAsync(int id);
    }
}
