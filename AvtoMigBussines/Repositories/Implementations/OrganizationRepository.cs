using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;
        public OrganizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Organization> GetPasswordrganization(double? password)
        {
            return await _context.Organizations.FirstOrDefaultAsync(x=>x.Password == password);
        }
        public async Task<Organization> GetByIdAsync(int? id)
        {
            return await _context.Organizations.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }
        public async Task<Organization> GetByNumberAsync(string number)
        {
            return await _context.Organizations.FirstOrDefaultAsync(x=>x.Number == number && x.IsDeleted == false);
        }
        public async Task<IEnumerable<Organization>> GetAllAsync()
        {
            return await _context.Organizations.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task AddAsync(Organization organization)
        {
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Organization organization)
        {
            _context.Organizations.Update(organization);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var organization = await GetByIdAsync(id);
            if (organization != null)
            {
                organization.IsDeleted = true;
                await UpdateAsync(organization);
            }
        }
        public async Task<bool> ExistsWithNumber(string number)
        {
            return await _context.Organizations.AnyAsync(c => c.Number == number && c.IsDeleted == false);
        }
    }
}
