using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<bool> CreateSubscriptionAsync(int? organizationId);
        Task UpdateSubscriptionAsync(int? organizationId);
        Task<Subscription> GetSubscriptionById(int organizationId);
    }
}
