using AvtoMigBussines.Models;

namespace AvtoMigBussines.Services.Interfaces
{
    public interface INotificationCenterService
    {
        Task<NotificationCenter> GetNotificationByIdAsync(int id);
        Task<IEnumerable<NotificationCenter>> GetAllNotificationsAsync(int? organizationId);
        Task<bool> CreateNotificationAsync(string message, string? aspNetUserId, string? title);
        Task UpdateNotificationAsync(NotificationCenter notification);
        Task DeleteNotificationAsync(int id);
    }
}
