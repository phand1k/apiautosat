using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.Data;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.CarWash.Repositories.Implementations
{
    public class WashServiceRepository : IWashServiceRepository
    {
        private readonly ApplicationDbContext _context;
        public WashServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(WashService washService)
        {
            _context.WashServices.Add(washService);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await GetByIdAsync(id);
            if (service != null)
            {
                service.IsDeleted = true;
                await UpdateAsync(service);
            }
        }

        public async Task<bool> ExistsWithName(int? orderId, int? serviceId)
        {
            return await _context.WashServices.
                Where(x => x.OrganizationId == orderId).
                AnyAsync(c => c.WashOrderId == orderId && (c.IsDeleted == false && c.ServiceId == serviceId));
        }

        public async Task<IEnumerable<WashService>> GetAllAsync()
        {
            return await _context.WashServices.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<WashService>> GetAllFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await _context.WashServices
                .Include(x => x.WashOrder.ModelCar.Car)
                .Where(x => x.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<WashService> GetByIdAsync(int id)
        {
            return await _context.WashServices.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task UpdateAsync(WashService washService)
        {
            _context.WashServices.Update(washService);
            await _context.SaveChangesAsync();
        }
    }
}
