using MFA.Core.Contract.IRepo;
using MFA.Core.Entities;
using MFA.Data;
using MFA.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MFA.Data.Repo
{

    public class ItUserMasterRepo : IItUserMasterRepo
    {
        private readonly ApplicationDBContext _context;

        public ItUserMasterRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ITUserMaster> AddItUserMaster(ITUserMaster user)
        {
            await _context.ITUserMasters.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateItUserMaster(ITUserMaster user)
        {
            _context.ITUserMasters.Update(user);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ITUserMaster> GetAllItUserMaster()
        {
            return _context.ITUserMasters.AsQueryable();
        }

        public async Task<ITUserMaster?> ValidateUser(string email, string password)
        {
            return await _context.Set<ITUserMaster>()
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<ITUserMaster?> GetByEmail(string email)
        {
            return await _context.Set<ITUserMaster>()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUser(ITUserMaster user)
        {
            _context.Set<ITUserMaster>().Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveOtpAsync(OtpMaster otp)
        {
            await _context.Otpmasters.AddAsync(otp);
            await _context.SaveChangesAsync();
        }
    }
}

