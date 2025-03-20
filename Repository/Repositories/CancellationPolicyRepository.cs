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
    public class CancellationPolicyRepository : BaseRepository<CancellationPolicy>, ICancellationPolicyRepository
    {
        private readonly CancellationPolicyDAO _canclepolicyDao;

        public CancellationPolicyRepository(CancellationPolicyDAO canclepolicyDao) : base(canclepolicyDao) 
        {
            _canclepolicyDao = canclepolicyDao;
        }

        // Thêm một CancellationPolicy
        public async Task<CancellationPolicy> AddAsync(CancellationPolicy entity)
        {
            return await _canclepolicyDao.AddAsync(entity);
        }

        // Thêm nhiều CancellationPolicy
        public async Task<List<CancellationPolicy>> AddRangeAsync(List<CancellationPolicy> entities)
        {
            return await _canclepolicyDao.AddRange(entities);
        }

        public async Task DeleteAsync(CancellationPolicy entity)
        {
            await _canclepolicyDao.DeleteAsync(entity); // Không cần trả về gì từ DAO, chỉ cần thực hiện xóa.
        }


        public async Task<List<CancellationPolicy>> DeleteRangeAsync(List<CancellationPolicy> entities)
        {
            return await _canclepolicyDao.DeleteRange(entities);
        }

        // Lấy tất cả CancellationPolicies
        public async Task<IEnumerable<CancellationPolicy>> GetAllAsync()
        {
            return await _canclepolicyDao.GetAllCancellationPoliciesAsync();
        }

        // Lấy CancellationPolicy theo ID
        public async Task<CancellationPolicy> GetByIdAsync(int id)
        {
            return await _canclepolicyDao.GetCancellationPolicyByIdAsync(id);
        }

        // Lấy CancellationPolicy theo một chuỗi ID (nếu cần)
        public async Task<CancellationPolicy> GetByStringIdAsync(string id)
        {
            return await _canclepolicyDao.GetByStringIdAsync(id); // Bạn cần thêm một phương thức cho chuỗi id nếu có.
        }

        // Cập nhật một CancellationPolicy
        public async Task<CancellationPolicy> UpdateAsync(CancellationPolicy entity)
        {
            return await _canclepolicyDao.UpdateAsync(entity);
        }

        // Cập nhật nhiều CancellationPolicy
        public async Task<List<CancellationPolicy>> UpdateRangeAsync(List<CancellationPolicy> entities)
        {
            return await _canclepolicyDao.UpdateRange(entities);
        }
    }
}
