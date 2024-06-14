using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;

namespace AvtoMigBussines.Services.Implementations
{
    public class ModelCarService : IModelCarService
    {
        private readonly IModelCarRepository _modelCarRepository;
        public ModelCarService(IModelCarRepository modelCarRepository)
        {
            _modelCarRepository = modelCarRepository;
        }
        public async Task<ModelCar> GetModelCarByIdAsync(int id)
        {
            return await _modelCarRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ModelCar>> GetAllModelCarsAsync(int id)
        {
            return await _modelCarRepository.GetAllAsync(id);
        }

        public async Task<bool> CreateModelCarAsync(ModelCar car)
        {
            // Проверяем, существует ли уже автомобиль с таким наименованием
            if (await _modelCarRepository.ExistsWithName(car.Name))
            {
                throw new Exception("Car with the same name already exists.");
            }

            await _modelCarRepository.AddAsync(car);
            return true;
        }

        public async Task UpdateModelCarAsync(ModelCar car)
        {
            await _modelCarRepository.UpdateAsync(car);
        }

        public async Task DeleteModelCarAsync(int id)
        {
            await _modelCarRepository.DeleteAsync(id);
        }
    }
}
