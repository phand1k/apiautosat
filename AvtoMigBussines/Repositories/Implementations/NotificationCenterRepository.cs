using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class NotificationCenterRepository : INotificationCenterRepository
    {
        private readonly ApplicationDbContext context;
        public NotificationCenterRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task AddAsync(NotificationCenter notificationCenter)
        {
            await context.NotificationCenters.AddAsync(notificationCenter);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int actionId, string actionType)
        {
            var notifications = context.NotificationCenters
            .Where(n => n.ActionId == actionId && n.ActionType == actionType)
            .ToList();
            foreach (var notification in notifications)
            {
                notification.IsDeleted = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsWithName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NotificationCenter>> GetAllAsync(int? organizationId)
        {
            return await context.NotificationCenters.OrderByDescending(x=>x.DateOfCreated).Where(p => p.IsDeleted == false && p.OrganizationId == organizationId).ToListAsync();
        }

        public async Task<NotificationCenter> GetByIdAsync(int id)
        {
            return await context.NotificationCenters.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task UpdateAsync(NotificationCenter notificationCenter)
        {
            context.NotificationCenters.Update(notificationCenter);
            await context.SaveChangesAsync();
        }
    }
}
