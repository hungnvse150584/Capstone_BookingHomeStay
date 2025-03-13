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
    public class CultureExperienceDAO : BaseDAO<CultureExperience>
    {
        private readonly GreenRoamContext _context;
        public CultureExperienceDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CultureExperience>> GetAllCommissionRateAsync()
        {
            return await _context.CultureExperiences
                        .ToListAsync();
        }
    }
}
