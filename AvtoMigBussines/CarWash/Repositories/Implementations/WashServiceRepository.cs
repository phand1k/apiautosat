using AvtoMigBussines.CarWash.Models;
using AvtoMigBussines.CarWash.Repositories.Interfaces;
using AvtoMigBussines.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvtoMigBussines.CarWash.Repositories.Implementations
{
    public class WashServiceRepository : IWashServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public WashServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WashService washService)
        {
            _context.WashServices.Add(washService);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await GetByIdAsync(id);
            if (service != null)
            {
                service.IsDeleted = true;
                await UpdateAsync(service);
            }
        }

        public async Task<bool> ExistsWithName(int? orderId, int? serviceId)
        {
            return await _context.WashServices
                .Where(x => x.OrganizationId == orderId)
                .AnyAsync(c => c.WashOrderId == orderId && (c.IsDeleted == false && c.ServiceId == serviceId));
        }

        public async Task<IEnumerable<WashService>> GetAllAsync()
        {
            return await _context.WashServices.Where(p => p.IsDeleted == false).ToListAsync();
        }
        public async Task<IEnumerable<WashService>> GetAllWashServicesWithPhoneNumber(string? phoneNumber)
        {
            return await _context.WashServices.Include(x=>x.Service).Include(x=>x.WashOrder.ModelCar.Car).Where(x=>x.WashOrder.PhoneNumber == phoneNumber).ToListAsync();
        }
        public async Task<IEnumerable<WashService>> GetAllFilterAsync(string? aspNetUserId, int? organizationId)
        {
            return await _context.WashServices.Include(x=>x.Service).Include(x=>x.AspNetUser)
                .Include(x => x.WashOrder.ModelCar.Car)
                .Where(x => x.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WashService>> GetAllMyNotCompletedWashServices(string? aspNetUserId)
        {
            return await _context.WashServices.Include(x => x.Service)
                .Include(x => x.WashOrder.ModelCar.Car).Include(x => x.AspNetUser).Where(x=> x.IsDeleted == false)
                .Where(x => x.WhomAspNetUserId == aspNetUserId && x.IsOvered == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<WashService>> GetAllMyWashServices(string? aspNetUserId)
        {
            return await _context.WashServices.Include(x => x.Service)
                .Include(x => x.WashOrder.ModelCar.Car).Include(x => x.AspNetUser).Where(x=>x.IsDeleted == false)
                .Where(x => x.WhomAspNetUserId == aspNetUserId && x.IsOvered == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<WashService>> GetAllWashServicesOnOrder(int? orderId)
        {
            return await _context.WashServices.Include(x => x.Service)
                .Where(x => x.WashOrderId == orderId && x.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<WashService>> GetAllNotCompletedWashServicesOnOrder(int? orderId)
        {
            return await _context.WashServices.Include(x => x.Service).Where(x => x.IsOvered == false)
                .Where(x => x.WashOrderId == orderId && x.IsDeleted == false).ToListAsync();
        }

        public async Task<WashService> GetByIdAsync(int id)
        {
            return await _context.WashServices.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }

        public async Task<int?> GetCountAllServices(int? orderId)
        {
            return await _context.WashServices.Where(x => x.WashOrderId == orderId && x.IsDeleted == false).CountAsync();
        }

        public async Task<double?> GetSummAllServices(int? orderId)
        {
            return await _context.WashServices.Where(x => x.WashOrderId == orderId && x.IsDeleted == false).Select(x => x.Price).SumAsync();
        }

        public async Task UpdateAsync(WashService washService)
        {
            _context.WashServices.Update(washService);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WashService>> GetAllServicesByWashOrderIdAsync(int id)
        {
            return await _context.WashServices.Where(x => x.IsOvered == false)
                .Where(x => x.WashOrderId == id && x.IsDeleted == false).ToListAsync();
        }

        public async Task<int?> GetAllNotCompletedServicesForNotCompeltedWashOrders(int? organizationId)
        {
            return await _context.WashServices.Where(x=>x.IsOvered == false && x.IsDeleted == false).CountAsync();
        }

        public async Task<int?> GettAllCompletedServicesForNotCompletedWashOrders(int? organizationId)
        {
            return await _context.WashServices.Include(x=>x.WashOrder).Where(x=>x.WashOrder.IsOvered == false).Where(x=>x.IsOvered == true && x.IsDeleted == false).CountAsync();
        }
        public async Task<double?> SummOfAllServicesOnNotCompletedWashOrders(int? organizationId)
        {
            return await _context.WashServices.
                Include(x=>x.WashOrder).Where(x=>x.WashOrder.IsDeleted == false && x.WashOrder.IsOvered == false).
                Where(x=>x.IsDeleted == false).Select(x=>x.Price).SumAsync();
        }
    }
}
