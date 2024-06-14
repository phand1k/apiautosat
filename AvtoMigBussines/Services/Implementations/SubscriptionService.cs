using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Implementations;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;

namespace AvtoMigBussines.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository subscriptionRepository;
        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            this.subscriptionRepository = subscriptionRepository;
        }
        public async Task<Subscription> GetSubscriptionById(int? organizationId)
        {
            return await subscriptionRepository.GetSubscriptionInfoAsync(organizationId);
        }
        public async Task<bool> CreateSubscriptionAsync(int? organizationId)
        {
            var subscription = new Subscription()
            {
                OrganizationId = organizationId
            };

            subscription.DateOfEnd = DateTime.Now.AddDays(14);
            await subscriptionRepository.CreateAsync(subscription);
            return true;
        }
        public async Task UpdateSubscriptionAsync(int? organizationId)
        {
            var subscription = new Subscription()
            {
                OrganizationId = organizationId
            };
            subscription.DateOfEnd = DateTime.Now.AddDays(30);
            await subscriptionRepository.UpdateAsync(subscription);
        }
    }
}
