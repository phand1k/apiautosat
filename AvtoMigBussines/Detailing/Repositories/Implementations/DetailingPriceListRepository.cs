using AvtoMigBussines.Data;
using AvtoMigBussines.Detailing.Models;
using AvtoMigBussines.Detailing.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Detailing.Repositories.Implementations
{
    public class DetailingPriceListRepository : IDetailingPriceListRepository
    {
        private ApplicationDbContext context;
        public DetailingPriceListRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> ExistsWithName(int? carId, int? modelCarId, int? serviceId, int? organizationId, double? price)
        {
            return await context.DetailingPriceLists.Where(x=>x.ServiceId == serviceId && x.CarId == carId).
                Where(x => x.OrganizationId == organizationId && x.Price == price).
                AnyAsync(c => c.ModelCarId == modelCarId && (c.IsDeleted == false));
        }

        public async Task AddAsync(DetailingPriceList detailingPriceList)
        {
            context.DetailingPriceLists.Add(detailingPriceList);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DetailingPriceList>> GetAllPriceListForService(int? serviceId, int? carId, int? modelCarId, int? organizationId)
        {
            return await context.DetailingPriceLists.
                Where(x => x.ServiceId == serviceId && x.OrganizationId == organizationId).
                Where(x => x.IsDeleted == false && x.CarId == carId).Where(x=>x.ModelCarId == modelCarId)
                .Include(x=>x.Service)
                .ToListAsync();
        }
        public async Task<IEnumerable<DetailingPriceList>> GetAllServiceList(int? serviceId, int? organizationId)
        {
            return await context.DetailingPriceLists.
                Where(x=>x.ServiceId == serviceId && x.OrganizationId == organizationId).Where(x=>x.IsDeleted == false).ToListAsync();
        }

        public Task<DetailingPriceList> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
