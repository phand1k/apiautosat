using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext context;
        public ServiceRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Service service)
        {
            context.Services.Add(service);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await context.Services.FirstOrDefaultAsync(x=>x.Id ==id);
            if (service != null)
            {
                service.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsWithName(string name, int? organizationId)
        {
            return await context.Services.Where(x=>x.OrganizationId == organizationId).AnyAsync(c => c.Name == name && c.IsDeleted == false);
        }

        public async Task<IEnumerable<Service>> GetAllAsync(int? organizationId)
        {
            return await context.Services.Where(x=>x.OrganizationId == organizationId && x.IsDeleted == false).ToListAsync();
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await context.Services.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public Task UpdateAsync(Service service)
        {
            throw new NotImplementedException();
        }
    }
}
