namespace AvtoMigBussines.Services.Interfaces
{
    public interface INotificationService
    {
        Task <bool> CheckAndNotifySubscriptionExpiryAsync(int organizationId);
    }
}
