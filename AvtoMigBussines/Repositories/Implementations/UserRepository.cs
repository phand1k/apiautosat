using AvtoMigBussines.Authenticate;
using AvtoMigBussines.Authenticate.Models;
using AvtoMigBussines.Data;
using AvtoMigBussines.Models;
using AvtoMigBussines.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AvtoMigBussines.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> userManager;
        public UserRepository(ApplicationDbContext context, UserManager<AspNetUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }
        public async Task<AspNetUser> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.AspNetUsers.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber && p.IsDeleted == false);
        }
        public async Task<AspNetUser> GetByIdAsync(string id)
        {
            return await _context.AspNetUsers.FirstOrDefaultAsync(p => p.UserName == id && p.IsDeleted == false);
        }
        public async Task<IEnumerable<AspNetUser>> GetAllAsync(int? organizationId)
        {
            return await _context.AspNetUsers.Where(p => p.IsDeleted == false && p.OrganizationId == organizationId).ToListAsync();
        }
        public async Task<bool> ConfirmForgotPassword(double? code, string? phoneNumber)
        {
            return await _context.ForgotPasswordCodes
                .Where(x => x.Code == code && x.PhoneNumber == phoneNumber && x.DateOfEnd > DateTime.UtcNow)
                .AnyAsync();
        }

        public async Task RegisterForgotPasswordCode(double code, string? phoneNumber)
        {
            ForgotPasswordCode forgotPasswordCode = new ForgotPasswordCode();
            forgotPasswordCode.Code = code;
            forgotPasswordCode.PhoneNumber = phoneNumber;
            forgotPasswordCode.DateOfEnd = forgotPasswordCode.DateOfCreated.Value.AddMinutes(20);
            await _context.ForgotPasswordCodes.AddAsync(forgotPasswordCode);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(AspNetUser aspNetUser)
        {
            await _context.AspNetUsers.AddAsync(aspNetUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AspNetUser aspNetUser)
        {
            _context.AspNetUsers.Update(aspNetUser);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await UpdateAsync(user);
            }
        }
        public async Task RegisterAsync(AspNetUser aspNetUser)
        {
            await _context.AspNetUsers.AddAsync(aspNetUser);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsWithPhoneNumber(string phoneNumber)
        {
            return await _context.AspNetUsers.AnyAsync(c => c.PhoneNumber == phoneNumber && c.IsDeleted == false);
        }
    }
}
