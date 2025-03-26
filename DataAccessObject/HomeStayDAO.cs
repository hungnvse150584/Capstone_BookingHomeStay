using BusinessObject.Model;
using BusinessObject.PaginatedLists;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class HomeStayDAO : BaseDAO<HomeStay>
    {
        private readonly GreenRoamContext _context;
        private const double EarthRadiusKm = 6371; // Bán kính trái đất (km)
        public HomeStayDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<HomeStay?> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateId = null)
        {
            var homestay = await _context.HomeStays.FindAsync(homestayId);
            if (homestay != null)
            {
                homestay.Status = status;
                await _context.SaveChangesAsync();
            }

            return await _context.HomeStays.FindAsync(homestayId);
        }

        public async Task<IEnumerable<HomeStay>> GetAllRegisterHomeStayAsync()
        {
            //return await _context.HomeStays
            //            .Include(c => c.Account)

            //            .ToListAsync();
            return await _context.HomeStays
               .Include(h => h.Account)
               .Include(h => h.Reports)
               .Include(h => h.HomeStayRentals)
               .Include(h => h.ImageHomeStays)
               .Include(h => h.Bookings)
               .Include(h => h.CultureExperiences)
               .Include(h => h.Services)
               .Include(h => h.Ratings)
               .ToListAsync();
        }

        public async Task<HomeStay> GetHomeStayDetailByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<HomeStay>()
                        .Include(c => c.Account)
               .SingleOrDefaultAsync(c => c.HomeStayID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<IEnumerable<HomeStay>> GetAllHomeStayAsync()
        {
            return await _context.HomeStays
                        .Include(c => c.Account)
                        .Include(c => c.Services)
                        .ToListAsync();
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<HomeStay> GetHomeStayByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<HomeStay>()
                        .Include(c => c.Account)
                        .Include(c => c.Services)
               .SingleOrDefaultAsync(c => c.HomeStayID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }
        public async Task<IEnumerable<HomeStay>> GetAllWithDetailsAsync()
        {
            return await _context.HomeStays
                .Include(h => h.HomeStayRentals)
                .Include(h => h.Bookings)
                    .ThenInclude(b => b.BookingDetails)
                .ToListAsync();
        }
        public async Task<HomeStay> GetOwnerHomeStayByIdAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException($"id {accountId} not found");
            }
            var entity = await _context.Set<HomeStay>()
                        .Include(c => c.Account)
               .FirstOrDefaultAsync(c => c.AccountID == accountId);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {accountId} not found");
            }
            return entity;
        }

        /*public async Task<IEnumerable<HomeStay>> SearchHomeStay(string? search)
        {

        }*/

        public async Task<IEnumerable<HomeStay>> GetNearestHomeStaysAsync(double userLat, double userLon, int pageIndex = 1, int pageSize = 5)
        {
            /*return await Task.Run(() =>
            {
                return _context.HomeStays
                    .AsEnumerable() // Chuyển sang xử lý trên RAM
                    .Select(hs => new
                    {
                        HomeStay = hs,
                        Distance = CalculateHaversineDistance(userLat, userLon, hs.Latitude, hs.Longitude)
                    })
                    .OrderBy(hs => hs.Distance)
                    .Take(topN)
                    .Select(hs => hs.HomeStay);
            });*/

            IQueryable<HomeStay> query = _context.HomeStays
                                        .Include(hs => hs.Account)
                                        .Include(hs => hs.ImageHomeStays)
                                        .Include(hs => hs.Ratings);

            var homeStaysWithDistance = query
                .AsEnumerable() // Chuyển sang xử lý trên RAM
                .Select(hs => new
                {
                    HomeStay = hs,
                    Distance = CalculateHaversineDistance(userLat, userLon, hs.Latitude, hs.Longitude)
                })
                .OrderBy(hs => hs.Distance)
                .Select(hs => hs.HomeStay)
                .AsQueryable(); // Chuyển về IQueryable để hỗ trợ phân trang

            return PaginatedList<HomeStay>.Create(homeStaysWithDistance, pageIndex, pageSize);
        }

        public double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c; // Khoảng cách tính bằng km
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
