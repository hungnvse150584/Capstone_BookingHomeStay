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
    public class RatingRepository : BaseRepository<Rating>, IRatingRepository
    {
        private readonly RatingDAO _ratingDao;

        public RatingRepository(RatingDAO ratingDao) : base(ratingDao)
        {
            _ratingDao = ratingDao;
        }


        public async Task<double> GetAverageRating(int homeStayId)
        {
            return await _ratingDao.GetAverageRating(homeStayId);
        }

        public async Task<Rating> GetByIdAsync(int id, bool includeAccount = false)
        {
            return await _ratingDao.GetByIdAsync(id, includeAccount);
        }

        public async Task<(IEnumerable<Rating> Data, int TotalCount)> GetRatingByHomeStayIdAsync(int homeStayId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10)
        {
            return await _ratingDao.GetRatingByHomeStayIdAsync(homeStayId, includeAccount, pageNumber, pageSize);
        }

        public async Task<(IEnumerable<Rating> Data, int TotalCount)> GetRatingByAccountIdAsync(string accountId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10)
        {
            return await _ratingDao.GetRatingByAccountIdAsync(accountId, includeAccount, pageNumber, pageSize);
        }

        public async Task<Rating?> GetRatingByUserIdAndHomeStay(string accountId, int homeStayId)
        {
           return await _ratingDao.GetRatingByUserIdAndHomeStay(accountId, homeStayId);
        }
        public async Task SaveChangesAsync()
        {
            await _ratingDao.SaveChangesAsync();
        }
        public async Task<(IEnumerable<Rating> Data, int TotalCount)> GetAllRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10)
        {
            return await _ratingDao.GetAllRatingByUserIdAndHomeStayAsync(accountId, homeStayId, includeAccount, pageNumber, pageSize);
        }
    }
}
