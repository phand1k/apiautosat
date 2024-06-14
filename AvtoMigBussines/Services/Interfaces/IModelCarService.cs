using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface IModelCarService
    {
        Task<ModelCar> GetModelCarByIdAsync(int id);
        Task<IEnumerable<ModelCar>> GetAllModelCarsAsync(int id);
        Task<bool> CreateModelCarAsync(ModelCar car);
        Task UpdateModelCarAsync(ModelCar car);
        Task DeleteModelCarAsync(int id);
    }
}
