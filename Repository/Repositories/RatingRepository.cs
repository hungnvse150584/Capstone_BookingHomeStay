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

        public async Task<IEnumerable<Rating?>> GetRatingByAccountId(string accountId)
        {
            return await _ratingDao.GetRatingByAccountId(accountId);
        }

        public async Task<IEnumerable<Rating?>> GetRatingByHomeStayId(int homeStayId)
        {
            return await _ratingDao.GetRatingByHomeStayId(homeStayId);
        }

        public async Task<Rating?> GetRatingByUserIdAndHomeStay(string accountId, int homeStayId)
        {
           return await _ratingDao.GetRatingByUserIdAndHomeStay(accountId, homeStayId);
        }
    }
}
