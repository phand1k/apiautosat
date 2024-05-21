using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface ICarService
    {
        Task<Car> GetCarByIdAsync(int id);
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<bool> CreateCarAsync(Car product);
        Task UpdateCarAsync(Car product);
        Task DeleteCarAsync(int id);
    }
}
