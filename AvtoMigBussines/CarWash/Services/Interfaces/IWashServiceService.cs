using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.DTOModels;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.CarWash.Services.Interfaces
{
    public interface IWashServiceService
    {
        Task<WashService> GetByIdAsync(int id);
        Task<IEnumerable<WashService>> GetAllAsync();
        Task<bool> CreateAsync(WashServiceDTO washServiceDTO, string aspNetUserId);
        Task UpdateAsync(WashService washService);
        Task DeleteAsync(int id);
    }
}
