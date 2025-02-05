using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        private readonly ReviewDAO _reviewDAO;

        public ReviewRepository(ReviewDAO reviewDAO) : base(reviewDAO)
        {
            _reviewDAO = reviewDAO;
        }

        public Task<Review> AddAsync(Review entity)
        {
            return _reviewDAO.AddAsync(entity);
        }

        public Task<Review> UpdateAsync(Review entity)
        {
            return _reviewDAO.UpdateAsync(entity);
        }

        public Task<Review> DeleteAsync(Review entity)
        {
            return _reviewDAO.DeleteAsync(entity);
        }

        public Task<IEnumerable<Review>> GetAllAsync()
        {
            return _reviewDAO.GetAllAsync();
        }

        public Task<IEnumerable<Review>> GetAllByHomeStayIdAsync(int homeStayId)
        {
            return _reviewDAO.GetAllByHomeStayIdAsync(homeStayId);
        }

        public Task<Review> GetReviewByIdAsync(int id)
        {
            return _reviewDAO.GetReviewByIdAsync(id);
        }
    }
}
