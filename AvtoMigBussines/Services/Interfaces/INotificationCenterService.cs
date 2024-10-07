using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface INotificationCenterService
    {
        Task<NotificationCenter> GetNotificationByIdAsync(int id);
        Task<IEnumerable<NotificationCenter>> GetAllNotificationsAsync(int? organizationId);
        Task<bool> CreateNotificationAsync(string message, string? aspNetUserId, string? title, int actionId, string actionType);
        Task UpdateNotificationAsync(NotificationCenter notification);
        Task DeleteNotificationAsync(int actionId, string actionType);
    }
}
