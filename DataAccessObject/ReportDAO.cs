using BusinessObject.Model;
using BusinessObject.PaginatedLists;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class ReportDAO : BaseDAO<Report>
    {
        private readonly GreenRoamContext _context;
        public ReportDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.Account)
                .Include(r => r.HomeStay)
                .Include(r => r.Booking)
                .ToListAsync();
        }

        public async Task<Report> GetReportByIdAsync(int id)
        {
            if (id == null || id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Report>()
               .Include(r => r.Account)
               .Include(r => r.HomeStay)
               .Include(r => r.Booking)
               .SingleOrDefaultAsync(r => r.ReportID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<IEnumerable<Report>> SearchReportAsync(string search, int pageIndex, int pageSize)
        {
            IQueryable<Report> searchReports = _context.Reports;

            if (!string.IsNullOrEmpty(search))
            {
                searchReports = searchReports
                            .Include(r => r.Account)
                            .Include(r => r.HomeStay)
                            .Include(r => r.Booking)
                            .Where(r => r.ReportText.ToLower().Contains(search.ToLower()));
            }

            var result = PaginatedList<Report>.Create(searchReports, pageIndex, pageSize).ToList();
            return result;
        }
    }
}
