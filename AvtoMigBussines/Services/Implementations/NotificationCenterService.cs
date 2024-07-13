using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using AvtoMigBussines.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AvtoMigBussines.Services.Implementations
{
    public class NotificationCenterService : INotificationCenterService
    {
        private readonly INotificationCenterRepository notificationCenterRepository;
        private readonly UserManager<AspNetUser> userManager;
        public NotificationCenterService(INotificationCenterRepository notificationCenterRepository, UserManager<AspNetUser> userManager)
        {
            this.notificationCenterRepository = notificationCenterRepository;
            this.userManager = userManager;
        }
        public async Task<bool> CreateNotificationAsync(string? message, string? aspNetUserId)
        {
            NotificationCenter notification = new NotificationCenter();
            var user = await userManager.FindByIdAsync(aspNetUserId);
            notification.OrganizationId = user.OrganizationId;
            notification.Title = "Завершен заказ-наряд";
            notification.Description = message;
            notification.AspNetUserId = aspNetUserId;
            await notificationCenterRepository.AddAsync(notification);
            return true;
        }

        public Task DeleteNotificationAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NotificationCenter>> GetAllNotificationsAsync()
        {
            return await notificationCenterRepository.GetAllAsync();
        }

        public Task<NotificationCenter> GetNotificationByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateNotificationAsync(NotificationCenter notification)
        {
            throw new NotImplementedException();
        }
    }
}
