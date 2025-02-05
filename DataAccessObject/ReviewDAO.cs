using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class ReviewDAO : BaseDAO<Review>
    {
        private readonly GreenRoamContext _context;

        public ReviewDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllByHomeStayIdAsync(int homeStayId)
        {
            return await _context.Reviews
                        .Where(r => r.HomeStayID == homeStayId)
                        .Include(r => r.Account) 
                        .Include(r => r.HomeStay) 
                        .ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"ID {id} không hợp lệ");
            }
            var entity = await _context.Reviews
                                       .Include(r => r.Account)
                                       .Include(r => r.HomeStay)
                                       .SingleOrDefaultAsync(r => r.ReviewID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Không tìm thấy Review với ID {id}");
            }
            return entity;
        }
    }
}
