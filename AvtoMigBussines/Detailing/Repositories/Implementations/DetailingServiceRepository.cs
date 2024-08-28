using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Data;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Detailing.Repositories.Implementations
{
    public class DetailingServiceRepository : IDetailingServiceRepository
    {
        private ApplicationDbContext context;
        public DetailingServiceRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<DetailingService>> GetAllDetailingServicesOnOrder(int? orderId)
        {
            return await context.DetailingServices.Include(x => x.Service).Include(x => x.AspNetUser)
                .Where(x => x.DetailingOrderId == orderId && x.IsDeleted == false).ToListAsync();
        }
        public async Task AddAsync(DetailingService detailingService)
        {
            context.DetailingServices.Add(detailingService);
            await context.SaveChangesAsync();
        }
        public async Task<double?> GetSummAllServices(int? orderId)
        {
            return await context.DetailingServices.Where(x => x.DetailingOrderId == orderId && x.IsDeleted == false).Select(x => x.Price).SumAsync();
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

        public Task<IEnumerable<DetailingService>> GetAllFilterAsync(string? aspNetUserId, int? organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task<DetailingService> GetByIdAsync(int id)
        {
            return await context.DetailingServices.Where(x=>x.IsDeleted == false).FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<IEnumerable<DetailingService>> GetAllServicesByDetailingOrderIdAsync(int id)
        {
            return await context.DetailingServices
               .Where(x => x.DetailingOrderId == id).ToListAsync();
        }

        public async Task UpdateAsync(DetailingService detailingService)
        {
            context.DetailingServices.Update(detailingService);
            await context.SaveChangesAsync();
        }

    }
}
