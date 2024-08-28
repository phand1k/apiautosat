using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface IClientService
    {
        Task SubscribeToUpdatesAsync(string carNumber, long telegramUserId);
        Task NotifyUsersAsync(string carNumber);
    }

}
