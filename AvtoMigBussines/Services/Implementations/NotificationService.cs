using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;

namespace AvtoMigBussines.Services.Implementations
{
    // NotificationService.cs
    public class NotificationService : INotificationService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationCenterService _notificationCenterService;

        public NotificationService(ISubscriptionRepository subscriptionRepository, INotificationCenterService notificationCenterService)
        {
            _subscriptionRepository = subscriptionRepository;
            _notificationCenterService = notificationCenterService;
        }

        public async Task<bool> CheckAndNotifySubscriptionExpiryAsync(int organizationId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByOrganizationIdAsync(organizationId);

            if (subscription != null && subscription.DateOfEnd.HasValue && subscription.DateOfEnd.Value < DateTime.Now)
            {
                // Подписка истекла
                return true;
            }

            // Подписка не истекла
            return false;
        }

    }

}
