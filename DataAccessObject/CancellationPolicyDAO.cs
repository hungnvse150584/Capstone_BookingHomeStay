using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class CancellationPolicyDAO : BaseDAO<CancellationPolicy>
    {
        private readonly GreenRoamContext _context;

        public CancellationPolicyDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CancellationPolicy>> GetAllCancellationPoliciesAsync()
        {
            return await _context.CancelPolicy.Include(cp => cp.HomeStay).ToListAsync();
        }

        public async Task<CancellationPolicy?> GetCancellationPolicyByIdAsync(int cancellationID)
        {
            return await _context.CancelPolicy.Include(cp => cp.HomeStay)
                                                      .FirstOrDefaultAsync(cp => cp.CancellationID == cancellationID);
        }

        public async Task<CancellationPolicy> AddAsync(CancellationPolicy cancellationPolicy)
        {
            await _context.CancelPolicy.AddAsync(cancellationPolicy);
            await _context.SaveChangesAsync();
            return cancellationPolicy;
        }

        public async Task<CancellationPolicy> UpdateAsync(CancellationPolicy cancellationPolicy)
        {
            _context.CancelPolicy.Update(cancellationPolicy);
            await _context.SaveChangesAsync();
            return cancellationPolicy;
        }

        public async Task DeleteAsync(CancellationPolicy cancellationPolicy)
        {
            _context.CancelPolicy.Remove(cancellationPolicy);
            await _context.SaveChangesAsync();
        }
    }
}
