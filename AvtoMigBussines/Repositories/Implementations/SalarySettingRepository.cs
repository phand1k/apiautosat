using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class SalarySettingRepository : ISalarySettingRepository
    {
        private readonly ApplicationDbContext context;
        public SalarySettingRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(SalarySetting salarySetting)
        {
            context.SalarySettings.Add(salarySetting);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var salarySetting = await GetByIdAsync(id);
            if (salarySetting != null)
            {
                salarySetting.IsDeleted = true;
                await UpdateAsync(salarySetting);
            }
        }

        public async Task<bool> ExistsSalarySetting(int? serviceId, string? aspNetUserId)
        {
            return await context.SalarySettings.Where(x => x.IsDeleted == false).AnyAsync(c => c.AspNetUserId == aspNetUserId && c.ServiceId == serviceId);
        }

        public async Task<IEnumerable<SalarySetting>> GetAllAsync(int? organizationId)
        {
            return await context.SalarySettings.Include(x=>x.Service).Include(x=>x.AspNetUser).
                Where(x=>x.IsDeleted == false && x.OrganizationId == organizationId).ToListAsync();
        }

        public async Task<SalarySetting> GetByIdAsync(int id)
        {
            return await context.SalarySettings.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task<SalarySetting> GetSalaryForUser(string? userId, int? serviceId)
        {
            return await context.SalarySettings.Where(x => x.ServiceId == serviceId && x.AspNetUserId == userId).FirstOrDefaultAsync(x => x.IsDeleted == false);
        }

        public async Task UpdateAsync(SalarySetting salarySetting)
        {
            throw new NotImplementedException();
        }
    }
}
