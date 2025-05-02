using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRatingRepository : IBaseRepository<Rating>
    {
        Task<double> GetAverageRating(int homeStayId);
        Task<IEnumerable<Rating?>> GetRatingByHomeStayId(int homeStayId);
        Task<IEnumerable<Rating?>> GetRatingByAccountId(string accountId);
        Task<Rating?> GetRatingByUserIdAndHomeStay(string accountId, int homeStayId);
        Task SaveChangesAsync();
    }
}
