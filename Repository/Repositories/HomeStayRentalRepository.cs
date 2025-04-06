using BusinessObject.Model;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class HomeStayRentalRepository : BaseRepository<HomeStayRentals>, IHomeStayRentalRepository
    {
        private readonly HomeStayRentalDAO _homestayrentalDao;

        public HomeStayRentalRepository(HomeStayRentalDAO homestayrentalDao) : base(homestayrentalDao)
        {
            _homestayrentalDao = homestayrentalDao;
        }

        public Task<HomeStayRentals> AddAsync(HomeStayRentals entity)
        {
            return _homestayrentalDao.AddAsync(entity);
        }

        public Task<HomeStayRentals> UpdateAsync(HomeStayRentals entity)
        {
            return _homestayrentalDao.UpdateAsync(entity);
        }

        public Task<HomeStayRentals> DeleteAsync(HomeStayRentals entity)
        {
            return _homestayrentalDao.DeleteAsync(entity);
        }

        public async Task<IEnumerable<HomeStayRentals>> GetAllAsyncFilter(bool? rentWhole = null)
        {
            return await _homestayrentalDao.GetAllHomeStayTypesAsyncFilter(null, rentWhole);
        }
        public Task<IEnumerable<HomeStayRentals>> GetAllAsync(int homestayId)
        {
            return _homestayrentalDao.GetAllHomeStayTypesAsync(homestayId);
        }

        public Task<HomeStayRentals> GetHomeStayTypesByIdAsync(int? id)
        {
            return _homestayrentalDao.GetHomeStayTypeByIdAsync(id);
        }

        public async Task<IEnumerable<HomeStayRentals>> GetHomeStayTypesByIdsAsync(List<int> homeStayTypeIds)
        {
            return await _homestayrentalDao.GetHomeStayTypesByIdsAsync(homeStayTypeIds);
        }

        public async Task SaveChangesAsync()
        {
            await _homestayrentalDao.SaveChangesAsync();
        }

        public async Task<IEnumerable<HomeStayRentals>> GetAllHomeStayTypesAsync(int homestayId)
        {
            return await _homestayrentalDao.GetAllHomeStayTypesAsync(homestayId);
        }

        public async Task AddRoomTypeAsync(RoomTypes roomType)
        {
            await _homestayrentalDao.AddRoomTypeAsync(roomType);
        }
        public async Task<IEnumerable<HomeStayRentals>> GetAllHomeStayTypesAsyncFilter(int? homestayId, bool? rentWhole = null)
        {
            return await _homestayrentalDao.GetAllHomeStayTypesAsyncFilter(homestayId, rentWhole);
        }
        public async Task<IEnumerable<HomeStayRentals>> FilterHomeStayRentalsAsync(
    int homeStayId,
    bool? rentWhole,
    DateTime checkInDate,
    DateTime checkOutDate,
    int numberOfAdults,
    int numberOfChildren)
        {
           
            bool rentWholeValue = rentWhole ?? false; 
            return await _homestayrentalDao.FilterHomeStayRentalsAsync(
                homeStayId,
                rentWholeValue,
                checkInDate,
                checkOutDate,
                numberOfAdults,
                numberOfChildren);
        }
    }
}
