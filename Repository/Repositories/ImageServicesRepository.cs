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

        public Task<ImageServices> AddAsync(ImageServices entity)
        {
            return _imageServicesDao.AddAsync(entity);
        }

        public Task<ImageServices> UpdateAsync(ImageServices entity)
        {
            return _imageServicesDao.UpdateAsync(entity);
        }

        public Task<ImageServices> DeleteAsync(ImageServices entity)
        {
            return _imageServicesDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<ImageServices>> GetAllAsync()   
        {
            return _imageServicesDao.GetAllAsync();
        }

        public Task<IEnumerable<ImageServices>> GetAllByServiceIdAsync(int serviceId)
        {
            return _imageServicesDao.GetAllByServiceIdAsync(serviceId);
        }

        public Task<ImageServices> GetImageServiceByIdAsync(int id)
        {
            return _imageServicesDao.GetImageServiceByIdAsync(id);
        }
    }
}
