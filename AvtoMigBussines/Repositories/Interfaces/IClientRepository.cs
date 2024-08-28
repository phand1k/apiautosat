using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetClientByCarNumberAsync(string carNumber);
        Task AddClientAsync(Client client);
        Task<IEnumerable<Client>> GetClientsByCarNumberAsync(string carNumber);
    }

}
