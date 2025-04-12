using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class StaffDAO : BaseDAO<Staff>
    {
        private readonly GreenRoamContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StaffDAO(GreenRoamContext context, UserManager<Account> userManager,
            RoleManager<IdentityRole> roleManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<Staff>> GetAllStaffByOwner(string accountID)
        {
            return await _context.Staffs
               .Include(h => h.Owner)
               .Include(h => h.HomeStay)
               .Where( s => s.AccountID == accountID)
               .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetAllStaffByHomeStay(int homeStayID)
        {
            return await _context.Staffs
               .Include(h => h.Owner)
               .Include(h => h.HomeStay)
               .Where(s => s.HomeStayID == homeStayID)
               .ToListAsync();
        }

        public async Task<Staff?> GetStaffByID(string accountID)
        {
            return await _context.Staffs
               .Include(h => h.Owner)
               .Include(h => h.HomeStay)
               .FirstOrDefaultAsync(h => h.StaffIdAccount == accountID);
        }
    }  
}
