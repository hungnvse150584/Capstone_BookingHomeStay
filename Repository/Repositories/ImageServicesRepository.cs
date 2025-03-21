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
    public class ImageServicesRepository : BaseRepository<ImageServices>, IImageServicesRepository
    {
        private readonly ImageServicesDAO _imageServicesDao;

        public ImageServicesRepository(ImageServicesDAO imageServicesDao) : base(imageServicesDao)
        {
            _imageServicesDao = imageServicesDao;
        }

        public async Task<ImageServices> AddAsync(ImageServices entity)
        {
            return  await _imageServicesDao.AddAsync(entity);
        }

        public async Task<ImageServices> UpdateAsync(ImageServices entity)
        {
            return await _imageServicesDao.UpdateAsync(entity);
        }

        public async Task<ImageServices> DeleteAsync(ImageServices entity)
        {
            return await _imageServicesDao.DeleteAsync(entity);
        }

        public async Task<IEnumerable<ImageServices>> GetAllAsync()   
        {
            return await _imageServicesDao.GetAllAsync();
        }

        public async Task<IEnumerable<ImageServices>> GetAllByServiceIdAsync(int serviceId)
        {
            return await _imageServicesDao.GetAllByServiceIdAsync(serviceId);
        }

        public async Task<ImageServices> GetImageServiceByIdAsync(int id)
        {
            return await _imageServicesDao.GetImageServiceByIdAsync(id);
        }

        public async Task<ImageServices> AddImageAsync(ImageServices image)
        {
            return await _imageServicesDao.AddAsync(image); ;
        }
    }
}
