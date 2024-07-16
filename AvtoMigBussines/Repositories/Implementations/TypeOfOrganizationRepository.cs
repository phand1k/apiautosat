using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
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
    }
}
