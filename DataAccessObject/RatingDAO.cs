using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject;

    public class RatingDAO : BaseDAO<Rating>
    {
        private readonly GreenRoamContext _context;
        public RatingDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<double> GetAverageRating(int homeStayId)
        {
            if (homeStayId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than zero.", nameof(homeStayId));
            }
            var ratings = await _context.Rating
                .Where(r => r.HomeStayID == homeStayId)
                .ToListAsync();

            if (ratings.Count == 0)
            {
                return 0;
            }

            var sum = ratings.Sum(r => r.Rate);
            var average = (double)sum / ratings.Count;

            return average;
        }

        public async Task<IEnumerable<Rating?>> GetRatingByHomeStayId(int homeStayId)
        {
            if (homeStayId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than zero.", nameof(homeStayId));
            }
            var entity = await _context.Rating
                .Where(r => r.HomeStayID == homeStayId)
                .ToListAsync();
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {homeStayId} not found");
            }
            return entity;
        }

        public async Task<IEnumerable<Rating?>> GetRatingByAccountId(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Account ID is required.", nameof(accountId));
            }
            var entity = await _context.Rating
                .Where(r => r.AccountID == accountId)
                .ToListAsync();
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {accountId} not found");
            }
            return entity;
        }

        public async Task<Rating?> GetRatingByUserIdAndHomeStay(string accountId, int homeStayId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Account ID is required.", nameof(accountId));
            }
            if (homeStayId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than zero.", nameof(homeStayId));
            }
            var entity = await _context.Rating
                .Where(r => r.AccountID == accountId && r.HomeStayID == homeStayId)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with accountid {accountId}, productId{homeStayId} not found");
            }
            return entity;
        }
    }

