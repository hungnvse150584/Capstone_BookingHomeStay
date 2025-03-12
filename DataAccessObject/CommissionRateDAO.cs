using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class CommissionRateDAO : BaseDAO<CommissionRate>
    {
        private readonly GreenRoamContext _context;
        public CommissionRateDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommissionRate>> GetAllCommissionRateAsync()
        {
            return await _context.CommissionRates
                        .Include(o => o.HomeStays)
                        .ToListAsync();
        }

        public async Task<CommissionRate?> GetCommissionRateByIDAsync(int rateID)
        {
            return await _context.CommissionRates
                .Include(o => o.HomeStays)
                .FirstOrDefaultAsync(o => o.CommissionRateID == rateID);
        }

        public async Task<CommissionRate?> GetCommissionRateByHomeStayAsync(int homeStayID)
        {
            var homeStay = await _context.HomeStays
                .Include(h => h.CommissionRate)
                .FirstOrDefaultAsync(h => h.HomeStayID == homeStayID);

            return homeStay?.CommissionRate;
        }
    }
}
