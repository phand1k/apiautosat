using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class ModelCarRepository : IModelCarRepository
    {
        private readonly ApplicationDbContext _context;
        public ModelCarRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ModelCar> GetByIdAsync(int id)
        {
            return await _context.ModelCars.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task<IEnumerable<ModelCar>> GetAllAsync(int id)
        {
            return await _context.ModelCars.
                Where(p => p.IsDeleted == false && p.CarId == id).OrderBy(x=>x.Name).ToListAsync();
        }

        public async Task AddAsync(ModelCar modelCar)
        {
            _context.ModelCars.Add(modelCar);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ModelCar modelCar)
        {
            _context.ModelCars.Update(modelCar);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var modelCar = await GetByIdAsync(id);
            if (modelCar != null)
            {
                modelCar.IsDeleted = true;
                await UpdateAsync(modelCar);
            }
        }
        public async Task<bool> ExistsWithName(string name)
        {
            return await _context.ModelCars.AnyAsync(c => c.Name == name && c.IsDeleted == false);
        }
    }
}
