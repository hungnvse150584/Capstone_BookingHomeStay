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
    public class ImageHomeStayRepository : BaseRepository<ImageHomeStay>, IImageHomeStayRepository
    {
        private readonly ImageHomeStayDAO _imageHomestayDao;

        public ImageHomeStayRepository(ImageHomeStayDAO homestayDao) : base(homestayDao)
        {
            _imageHomestayDao = homestayDao;
        }
        public Task<ImageHomeStay> AddAsync(ImageHomeStay entity)
        {
            return _imageHomestayDao.AddAsync(entity);
        }

        public async Task<ImageHomeStay> AddImageAsync(ImageHomeStay image)
        {
            return await _imageHomestayDao.AddAsync(image); ;
        }

        public Task<List<ImageHomeStay>> AddRange(List<ImageHomeStay> entities)
        {
            return _imageHomestayDao.AddRange(entities);
        }

        public Task<ImageHomeStay> DeleteAsync(ImageHomeStay entity)
        {
            return _imageHomestayDao.DeleteAsync(entity);
        }

        public Task<List<ImageHomeStay>> DeleteRange(List<ImageHomeStay> entities)
        {
            return _imageHomestayDao.DeleteRange(entities);
        }

        public Task<IEnumerable<ImageHomeStay>> GetAllAsync()
        {
            return _imageHomestayDao.GetAllAsync();
        }

        public Task<ImageHomeStay> GetByIdAsync(int id)
        {
            return _imageHomestayDao.GetByIdAsync(id);
        }

        public Task<ImageHomeStay> GetByStringId(string id)
        {
            return _imageHomestayDao.GetByStringIdAsync(id);
        }

        public Task<ImageHomeStay> UpdateAsync(ImageHomeStay entity)
        {
            return _imageHomestayDao.UpdateAsync(entity);
        }

        public Task<List<ImageHomeStay>> UpdateRange(List<ImageHomeStay> entities)
        {
            return _imageHomestayDao.UpdateRange(entities);
        }
        public async Task DeleteImageAsync(ImageHomeStay image)
        {
            await _imageHomestayDao.DeleteAsync(image);
        }

        public async Task SaveChangesAsync()
        {
            await _imageHomestayDao.SaveChangesAsync();
        }

        public async Task<IEnumerable<ImageHomeStay>> GetImagesByHomeStayIdAsync(int homeStayId)
        {
            return await _imageHomestayDao.GetImagesByHomeStayIdAsync(homeStayId);
        }
        public async Task<ImageHomeStay> UpdateImageAsync(ImageHomeStay image)
        {
            return await _imageHomestayDao.UpdateImageAsync(image);
        }

    }
}
