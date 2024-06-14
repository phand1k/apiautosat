using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task CreateAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task<Subscription> GetSubscriptionInfoAsync(int? id);
    }
}
