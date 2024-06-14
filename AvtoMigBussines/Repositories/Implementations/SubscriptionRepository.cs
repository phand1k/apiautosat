using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext context;
        public SubscriptionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task CreateAsync(Subscription subscription)
        {
            context.Subscriptions.Add(subscription);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Subscription subscription)
        {
            context.Update(subscription);
            await context.SaveChangesAsync();
        }
        public async Task<Subscription> GetSubscriptionInfoAsync(int? id)
        {
            return await context.Subscriptions.Include(x=>x.Organization).OrderByDescending(p=>p.DateOfCreated).LastOrDefaultAsync(p =>p.IsDeleted == false && p.OrganizationId == id);
        }
    }
}
