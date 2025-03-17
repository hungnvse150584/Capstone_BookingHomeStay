using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class HomeStayDAO : BaseDAO<HomeStay>
    {
        private readonly GreenRoamContext _context;
        public HomeStayDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<HomeStay?> ChangeHomeStayStatus(int homestayId, HomeStayStatus status)
        {
            var homestay = await _context.HomeStays.FindAsync(homestayId);
            if (homestay != null)
            {
                homestay.Status = status;
                await _context.SaveChangesAsync();
            }

            return await _context.HomeStays.FindAsync(homestayId);
        }

        public async Task<IEnumerable<HomeStay>> GetAllRegisterHomeStayAsync()
        {
            return await _context.HomeStays
                        .Include(c => c.Account)
                        .ToListAsync();
        }

        public async Task<HomeStay> GetHomeStayDetailByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<HomeStay>()
                        .Include(c => c.Account)
               .SingleOrDefaultAsync(c => c.HomeStayID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<IEnumerable<HomeStay>> GetAllHomeStayAsync()
        {
            return await _context.HomeStays
                        .Include(c => c.Account)
                        .Include(c => c.Services)
                        .ToListAsync();
        }

        public async Task<HomeStay> GetHomeStayByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<HomeStay>()
                        .Include(c => c.Account)
                        .Include(c => c.Services)
               .SingleOrDefaultAsync(c => c.HomeStayID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<HomeStay> GetOwnerHomeStayByIdAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException($"id {accountId} not found");
            }
            var entity = await _context.Set<HomeStay>()
                        .Include(c => c.Account)
               .FirstOrDefaultAsync(c => c.AccountID == accountId);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {accountId} not found");
            }
            return entity;
        }

        /*public async Task<IEnumerable<HomeStay>> SearchHomeStay(string? search)
        {

        }*/
    }
}
