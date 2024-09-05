using System.Collections.Generic;
using AvtoMigBussines.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AvtoMigBussines.Authenticate;
using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.Detailing.Models;

namespace AvtoMigBussines.Data
{
    public class ApplicationDbContext : IdentityDbContext<AspNetUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<DetailingOrderTransaction> DetailingOrderTransactions { get; set; }
        public DbSet<DetailingService> DetailingServices { get; set; }
        public DbSet<DetailingPriceList> DetailingPriceLists { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<TypeOfOrganization> TypeOfOrganizations { get; set; }
        public DbSet<DetailingOrder> DetailingOrders { get; set; }
        public DbSet<NotificationCenter> NotificationCenters { get; set; }
        public DbSet<WashOrderTransaction> WashOrderTransactions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<SalarySetting> SalarySettings { get; set; }
        public DbSet<WashService> WashServices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<NotifiactionToken> NotifiactionTokens { get; set; }
        public DbSet<WashOrder> WashOrders { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ModelCar> ModelCars { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
