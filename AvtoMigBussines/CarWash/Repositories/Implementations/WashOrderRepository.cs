using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.CarWash.Repositories.Implementations
{
    public class WashOrderRepository: IWashOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public WashOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<WashOrder> GetByIdAsync(int id)
        {
            return await _context.WashOrders.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }
        public async Task<IEnumerable<WashOrder>> GetAllAsync()
        {
            return await _context.WashOrders.Where(p => p.IsDeleted == false).ToListAsync();
        }
        public async Task<IEnumerable<WashOrder>> GetAllFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await _context.WashOrders
                .Include(x => x.ModelCar.Car)
                .Where(x => x.AspNetUserId == aspNetUserId && x.OrganizationId == organizationId)
                .OrderByDescending(x => x.DateOfCreated)
                .Take(5)
                .ToListAsync();
        }

        public async Task AddAsync(WashOrder carWashOrder)
        {
            _context.WashOrders.Add(carWashOrder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WashOrder carWashOrder)
        {
            _context.WashOrders.Update(carWashOrder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                await UpdateAsync(product);
            }
        }
        public async Task<bool> ExistsWithName(string carNumber, int? organizationId)
        {
            return await _context.WashOrders.Where(x=>x.OrganizationId == organizationId).AnyAsync(c => c.CarNumber == carNumber && (c.IsDeleted == false && c.IsOvered == false ));
        }
    }
}
