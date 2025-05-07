using BusinessObject.Model;
using DataAccessObject;
using Microsoft.Identity.Client;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        private readonly StaffDAO _staffDao;

        public StaffRepository(StaffDAO staffDao) : base(staffDao)
        {
            _staffDao = staffDao;
        }

        public async Task<Staff> AddAsync(Staff entity)
        {
            return await _staffDao.AddAsync(entity);
        }

        public async Task<IEnumerable<Staff>> GetAllStaffByHomeStay(int homeStayID)
        {
            return await _staffDao.GetAllStaffByHomeStay(homeStayID);
        }

        public async Task<IEnumerable<Staff>> GetAllStaffByOwner(string accountID)
        {
            return await _staffDao.GetAllStaffByOwner(accountID);
        }

        public async Task<Staff> GetByStaffIdAccountAsync(string staffIdAccount)
        {
            return await _staffDao.GetByStaffIdAccountAsync(staffIdAccount);
        }

        public async Task<List<Staff>> GetStaffByHomeStayIdAsync(int homeStayId)
        {
            return await _staffDao.GetStaffByHomeStayIdAsync(homeStayId);
        }

        public async Task<Staff?> GetStaffByID(string accountID)
        {
            return await _staffDao.GetStaffByID(accountID);
        }

        public async Task<Staff> UpdateAsync(Staff entity)
        {
            return await _staffDao.UpdateAsync(entity);
        }
    }
}
