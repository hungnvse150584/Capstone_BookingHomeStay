using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        private readonly ReportDAO _reportDao;
        public ReportRepository(ReportDAO reportDao) : base(reportDao)
        {
            _reportDao = reportDao;
        }

        public Task<IEnumerable<Report>> SearchReportAsync(string search, int pageIndex, int pageSize)
        {
            return _reportDao.SearchReportAsync(search, pageIndex, pageSize);
        }

        public async Task<Report> GetReportByIdAsync(int id)
        {
            return await _reportDao.GetReportByIdAsync(id);
        }

        public async Task<Report> AddAsync(Report entity)
        {
            return await _reportDao.AddAsync(entity);
        }

        public async Task<Report> UpdateAsync(Report entity)
        {
            return await _reportDao.UpdateAsync(entity);
        }

        public async Task<Report> DeleteAsync(Report entity)
        {
            return await _reportDao.DeleteAsync(entity);
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _reportDao.GetAllReportsAsync();
        }
    }
}
