using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRatingRepository : IBaseRepository<Rating>
    {
       
        Task<double> GetAverageRating(int homeStayId);
 
        Task<Rating?> GetRatingByUserIdAndHomeStay(string accountId, int homeStayId);
        Task SaveChangesAsync();
        Task<Rating> GetByIdAsync(int id, bool includeAccount = false);
        Task<(IEnumerable<Rating> Data, int TotalCount)> GetRatingByHomeStayIdAsync(int homeStayId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10);
        Task<(IEnumerable<Rating> Data, int TotalCount)> GetRatingByAccountIdAsync(string accountId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10);
        Task<(IEnumerable<Rating> Data, int TotalCount)> GetAllRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10);
    }
}
