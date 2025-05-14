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
                   .Include(h => h.CommissionRate)
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
                .Include(h => h.Reports)
                .Include(h => h.ImageHomeStays)
                .Include(h => h.CultureExperiences)
                .Include(h => h.CommissionRate)
                .Include(h => h.Services)
                .Include(h => h.Ratings)
                    .ThenInclude(r => r.ImageRatings)
                .Include(h => h.Ratings)
                    .ThenInclude(r => r.Account)
                .Include(h => h.HomeStayRentals)
                    .ThenInclude(r => r.RoomTypes)
                        .ThenInclude(rt => rt.Prices)
                .Include(h => h.CancelPolicy)
                .SingleOrDefaultAsync(c => c.HomeStayID == id);

            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }

            // Log để debug
            Console.WriteLine($"Ratings count for HomeStayID {id}: {(entity.Ratings != null ? entity.Ratings.Count : 0)}");
            if (entity.Ratings != null)
            {
                foreach (var rating in entity.Ratings)
                {
                    Console.WriteLine($"RatingID: {rating.RatingID}, SumRate: {rating.SumRate}");
                }
            }
            Console.WriteLine($"HomeStayRentals count for HomeStayID {id}: {(entity.HomeStayRentals != null ? entity.HomeStayRentals.Count : 0)}");
            if (entity.HomeStayRentals != null && entity.HomeStayRentals.Any())
            {
                foreach (var rental in entity.HomeStayRentals)
                {
                    Console.WriteLine($"RoomTypes count for HomeStayRentalID {rental.HomeStayRentalID}: {(rental.RoomTypes != null ? rental.RoomTypes.Count : 0)}");
                    if (rental.RoomTypes != null && rental.RoomTypes.Any())
                    {
                        foreach (var roomType in rental.RoomTypes)
                        {
                            Console.WriteLine($"Prices count for RoomTypeID {roomType.RoomTypesID}: {(roomType.Prices != null ? roomType.Prices.Count : 0)}");
                            if (roomType.Prices != null)
                            {
                                foreach (var price in roomType.Prices)
                                {
                                    Console.WriteLine($"PriceID: {price.PricingID}, RentPrice: {price.RentPrice}, IsDefault: {price.IsDefault}, IsActive: {price.IsActive}");
                                }
                            }
                        }
                    }
                }
            }

            return entity;
        }
        private async Task<CancellationPolicy> GetCancellationPolicyByHomeStayIdAsync(int homeStayId)
            {
                // Lấy CancellationPolicy mới nhất dựa trên CreateAt hoặc UpdateAt
                return await _context.CancelPolicy
                    .Where(cp => cp.HomeStayID == homeStayId)
                    .OrderByDescending(cp => cp.UpdateAt) // Sắp xếp theo UpdateAt giảm dần
                    .FirstOrDefaultAsync(); // Lấy cái đầu tiên (mới nhất)
            }
            public async Task<IEnumerable<HomeStay>> GetAllHomeStayAsync()
            {
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
                        .ThenInclude(r => r.RoomTypes)
                            .ThenInclude(rt => rt.Rooms)
                    .Include(h => h.HomeStayRentals) 
            .ThenInclude(r => r.RoomTypes)
                .ThenInclude(rt => rt.Prices)
                    .Include(h => h.Bookings)
                        .ThenInclude(b => b.BookingDetails)
                    .Include(h => h.ImageHomeStays)
                    .Include(h => h.Ratings)
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

            public async Task<List<(Account Account, int TotalHomeStays)>> GetOwnersWithHomeStayStatsAsync()
            {
                // Ép EF thực hiện query HomeStays trước (chạy trên SQL)
                var homeStayStats = await _context.HomeStays
                    .GroupBy(h => h.AccountID)
                    .Select(g => new
                    {
                        OwnerId = g.Key,
                        TotalHomeStays = g.Count(),
                        LatestCreatedDate = g.Max(h => h.CreateAt)
                    })
                    .ToListAsync(); // Dữ liệu đã nằm client-side, giờ join thoải mái

                // Join ở đây là LINQ thuần (in-memory)
                var result = homeStayStats
                    .Join(_context.Accounts.AsEnumerable(),
                          stat => stat.OwnerId,
                          acc => acc.Id,
                          (stat, acc) => new
                          {
                              Account = acc,
                              stat.TotalHomeStays,
                              stat.LatestCreatedDate
                          })
                    .OrderByDescending(x => x.LatestCreatedDate)
                    .Select(x => (x.Account, x.TotalHomeStays))
                    .ToList();

                return result;
            }

            public async Task<List<(HomeStay HomeStay, double AverageRating, int RatingCount)>> GetTrendingHomeStaysAsync(int top = 10)
            {
                // Bước 1: Load data trước từ EF
                var homeStays = await _context.HomeStays
                    .Include(h => h.Ratings)
                    .Include(h => h.ImageHomeStays)
                    .Where(h => h.Status == HomeStayStatus.Accepted)
                    .Where(h => h.Ratings.Any())
                    .ToListAsync();

                // Bước 2: In-memory processing
                var result = homeStays
                    .Select(h => (
                        HomeStay: h,
                        AverageRating: h.Ratings.Average(r => r.SumRate),
                        RatingCount: h.Ratings.Count()
                    ))
                    .OrderByDescending(x => x.AverageRating)
                    .ThenByDescending(x => x.RatingCount)
                    .Take(top)
                    .ToList();

                return result;
            }

            public async Task<List<(string accountID, string ownerName, int totalHomeStays)>> GetTopLoyalOwnersAsync(int top = 5)
            {
                var topOwners = await _context.HomeStays
                    .GroupBy(h => new { h.AccountID, h.Account.Name })
                    .Select(g => new
                    {
                        AccountId = g.Key.AccountID,
                        OwnerName = g.Key.Name,
                        TotalHomeStays = g.Count()
                    })
                    .OrderByDescending(x => x.TotalHomeStays)
                    .Take(top)
                    .ToListAsync();

                return topOwners
                      .Select(x => (x.AccountId, x.OwnerName, x.TotalHomeStays))
                      .ToList();
            }
        }
    }
