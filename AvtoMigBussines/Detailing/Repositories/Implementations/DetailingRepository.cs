using AvtoMigBussines.Data;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Detailing.Repositories.Implementations
{
    public class DetailingRepository : IDetailingRepository
    {
        private readonly ApplicationDbContext context;
        public DetailingRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(DetailingOrder detailingOrder)
        {
            context.DetailingOrders.Add(detailingOrder);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                order.IsDeleted = true;
                await UpdateAsync(order);
            }
        }

        public async Task<bool> ExistsWithName(string carNumber, int? organizationId)
        {
            return await context.DetailingOrders.Where(x => x.OrganizationId == organizationId).AnyAsync(c => c.CarNumber == carNumber && (c.IsDeleted == false && c.IsOvered == false));
        }

        public async Task<IEnumerable<DetailingOrder>> GetAllFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await context.DetailingOrders
                 .Include(x => x.ModelCar.Car)
                 .Where(x => x.AspNetUserId == aspNetUserId && x.OrganizationId == organizationId)
                 .OrderByDescending(x => x.DateOfCreated)
                 .Take(5)
                 .ToListAsync();
        }

        public async Task<DetailingOrder> GetByIdAsync(int id)
        {
            return await context.DetailingOrders.Include(x => x.ModelCar.Car).Include(x => x.AspNetUser).FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public Task UpdateAsync(DetailingOrder detailingOrder)
        {
            throw new NotImplementedException();
        }
    }
}
