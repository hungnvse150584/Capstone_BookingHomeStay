using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<Report> GetReportByIdAsync(int id);
        Task<IEnumerable<Report>> SearchReportAsync(string search, int pageIndex, int pageSize);
    }
}
