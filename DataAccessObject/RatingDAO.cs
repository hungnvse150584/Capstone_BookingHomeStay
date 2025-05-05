using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public async Task<Rating> GetByIdAsync(int id, bool includeAccount = false)
    {
        var query = _context.Rating.AsQueryable();
        if (includeAccount)
        {
            query = query.Include(r => r.Account);
        }
        return await query.FirstOrDefaultAsync(r => r.RatingID == id);
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

            var sum = ratings.Sum(r => r.SumRate);
            var average = (double)sum / ratings.Count;

            return average;
        }

    public async Task<(IEnumerable<Rating> Data, int TotalCount)> GetRatingByHomeStayIdAsync(int homeStayId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Rating.AsQueryable();
        if (includeAccount)
        {            query = query.Include(r => r.Account);
        }
        query = query.Where(r => r.HomeStayID == homeStayId);
        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (data, totalCount);
    }

    public async Task<(IEnumerable<Rating> Data, int TotalCount)> GetRatingByAccountIdAsync(string accountId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Rating.AsQueryable();
        if (includeAccount)
        {
            query = query.Include(r => r.Account);
        }
        query = query.Where(r => r.AccountID == accountId);
        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (data, totalCount);
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
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    public async Task<(IEnumerable<Rating> Data, int TotalCount)> GetAllRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId, bool includeAccount = false, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Rating.AsQueryable();
        if (includeAccount)
        {
            query = query.Include(r => r.Account);
        }
        query = query.Where(r => r.AccountID == accountId && r.HomeStayID == homeStayId);
        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (data, totalCount);
    }

}

