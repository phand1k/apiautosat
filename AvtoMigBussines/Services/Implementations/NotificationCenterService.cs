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
        public async Task<bool> CreateNotificationAsync(string? message, string? aspNetUserId, string? title, int actionId, string actionType)
        {
            NotificationCenter notification = new NotificationCenter();
            var user = await userManager.FindByIdAsync(aspNetUserId);
            notification.OrganizationId = user.OrganizationId;
            notification.Title = title;
            notification.Description = message;
            notification.AspNetUserId = aspNetUserId;
            notification.ActionId = actionId;
            notification.ActionType = actionType;
            await notificationCenterRepository.AddAsync(notification);
            return true;
        }

        public async Task DeleteNotificationAsync(int actionId, string actionType)
        {
            await notificationCenterRepository.DeleteAsync(actionId, actionType);
        }

        public async Task<IEnumerable<NotificationCenter>> GetAllNotificationsAsync(int? organizationId)
        {
            return await notificationCenterRepository.GetAllAsync(organizationId);
        }

        public async Task<NotificationCenter> GetNotificationByIdAsync(int id)
        {
            return await notificationCenterRepository.GetByIdAsync(id);
        }

        public async Task UpdateNotificationAsync(NotificationCenter notification)
        {
            await notificationCenterRepository.UpdateAsync(notification);
        }
    }
}
