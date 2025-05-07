using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IHomeStayRepository : IBaseRepository<HomeStay>
    {
        Task<HomeStay?> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateID = null);
        Task<IEnumerable<HomeStay>> GetAllRegisterHomeStayAsync();
        Task<HomeStay> GetHomeStayDetailByIdAsync(int id);
        Task<HomeStay> GetOwnerHomeStayByIdAsync(string accountId);
        /*Task<IEnumerable<HomeStay>> GetNearestHomeStaysAsync(double userLat, double userLon, int topN = 5);*/
        Task<IEnumerable<HomeStay>> GetNearestHomeStaysAsync(double userLat, double userLon, int pageIndex = 1, int pageSize = 5);
        Task<double> CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2);
        Task SaveChangesAsync();
        Task<IEnumerable<HomeStay>> GetAllWithDetailsAsync();
        Task<List<(Account Account, int TotalHomeStays)>> GetOwnersWithHomeStayStats();
        Task<List<(HomeStay HomeStay, double AverageRating, int RatingCount)>> GetTrendingHomeStaysAsync(int top = 10);
    }
}
