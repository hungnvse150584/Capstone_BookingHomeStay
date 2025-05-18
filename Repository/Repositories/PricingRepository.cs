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
    public class PricingRepository : BaseRepository<Pricing>, IPricingRepository
    {
        private readonly PricingDAO _pricingDao;

        public PricingRepository(PricingDAO pricingDao) : base(pricingDao)
        {
            _pricingDao = pricingDao;
        }

        public async Task<IEnumerable<Pricing>> GetAllPricingByHomeStayAsync(int homestayID)
        {
            return await _pricingDao.GetAllPricingByHomeStayAsync(homestayID);
        }

        public async Task<IEnumerable<Pricing>> GetPricingByHomeStayRentalAsync(int rentalID)
        {
            return await _pricingDao.GetPricingByHomeStayRentalAsync(rentalID);
        }

        public async Task<Pricing> GetPricingByIdAsync(int id)
        {
            return await _pricingDao.GetPricingByIdAsync(id);
        }

        public async Task<IEnumerable<Pricing>> GetPricingByRoomTypeAsync(int roomTypeID)
        {
           return await _pricingDao.GetPricingByRoomTypeAsync(roomTypeID);
        }

        public async Task<Pricing> AddAsync(Pricing entity)
        {
            return await _pricingDao.AddAsync(entity);
        }

        public async Task<Pricing> UpdateAsync(Pricing entity)
        {
            return await _pricingDao.UpdateAsync(entity);
        }

        public async Task<Pricing> DeleteAsync(Pricing entity)
        {
            return await _pricingDao.DeleteAsync(entity);
        }

        public async Task<DayType> GetDayType(DateTime date, int? homeStayRentalId, int? roomtypeId)
        {
            return await _pricingDao.GetDayType(date, homeStayRentalId, roomtypeId);
        }

        public async Task<double> GetTotalPrice(DateTime checkInDate, DateTime checkOutDate, int? homeStayRentalId, int? roomTypeId)
        {
            return await _pricingDao.GetTotalPrice(checkInDate, checkOutDate, homeStayRentalId, roomTypeId);
        }

        public async Task SaveChangesAsync()
        {
            await _pricingDao.SaveChangesAsync();
        }

        public async Task<List<Pricing>> GetPricingDetailsToRemoveAsync(int homeStayRentalID, List<int> updatedDetailIds)
        {
            return await _pricingDao.GetPricingDetailsToRemoveAsync(homeStayRentalID, updatedDetailIds);
        }
    }
}
