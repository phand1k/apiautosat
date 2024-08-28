using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Client?> GetClientByCarNumberAsync(string carNumber)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.CarNumber == carNumber && c.IsDeleted == false);
        }

        public async Task AddClientAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Client>> GetClientsByCarNumberAsync(string carNumber)
        {
            return await _context.Clients
                .Where(c => c.CarNumber == carNumber && c.IsDeleted == false)
                .ToListAsync();
        }
    }

}
