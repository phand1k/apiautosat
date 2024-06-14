using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface IModelCarRepository
    {
        Task <ModelCar> GetByIdAsync(int id);
        Task<IEnumerable<ModelCar>> GetAllAsync(int id);
        Task AddAsync(ModelCar car);
        Task UpdateAsync(ModelCar car);
        Task DeleteAsync(int id);
        Task<bool> ExistsWithName(string name);
    }
}
