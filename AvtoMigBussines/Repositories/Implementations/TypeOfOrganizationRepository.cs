using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class TypeOfOrganizationRepository : ITypeOfOrganizationRepository
    {
        private readonly ApplicationDbContext context;
        public TypeOfOrganizationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<TypeOfOrganization>> GetAllAsync()
        {
            return await context.TypeOfOrganizations.Where(p => p.IsDeleted == false).ToListAsync();
        }
        public async Task<string> GetByIdAsync(int? id)
        {
            return await context.Organizations.Include(x=>x.TypeOfOrganization).Where(x=>x.Id == id && x.IsDeleted == false).Select(x=>x.TypeOfOrganization.Name).FirstOrDefaultAsync();
        }
    }
}
