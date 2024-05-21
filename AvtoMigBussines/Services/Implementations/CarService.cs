using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;

namespace AvtoMigBussines.Services.Implementations
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {
            return await _carRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _carRepository.GetAllAsync();
        }

        public async Task<bool> CreateCarAsync(Car car)
        {
            // Проверяем, существует ли уже автомобиль с таким наименованием
            if (await _carRepository.ExistsWithName(car.Name))
            {
                throw new Exception("Car with the same name already exists.");
            }

            await _carRepository.AddAsync(car);
            return true;
        }

        public async Task UpdateCarAsync(Car car)
        {
            await _carRepository.UpdateAsync(car);
        }

        public async Task DeleteCarAsync(int id)
        {
            await _carRepository.DeleteAsync(id);
        }
    }
}
