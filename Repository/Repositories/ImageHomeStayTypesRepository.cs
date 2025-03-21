using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ImageHomeStayTypesRepository : BaseRepository<ImageHomeStayRentals>, IImageHomeStayTypesRepository
    {
        private readonly ImageHomeStayRentalsDAO _imageHomeStayTypesDAO;
        public ImageHomeStayTypesRepository(ImageHomeStayRentalsDAO imageHomeStayTypesDAO) : base(imageHomeStayTypesDAO)
        {
            _imageHomeStayTypesDAO = imageHomeStayTypesDAO;
        }
        public Task<ImageHomeStayRentals> AddAsync(ImageHomeStayRentals entity)
        {
            return _imageHomeStayTypesDAO.AddAsync(entity);
        }

        public Task<ImageHomeStayRentals> UpdateAsync(ImageHomeStayRentals entity)
        {
            return _imageHomeStayTypesDAO.UpdateAsync(entity);
        }

        public Task<ImageHomeStayRentals> DeleteAsync(ImageHomeStayRentals entity)
        {
            return _imageHomeStayTypesDAO.DeleteAsync(entity);
        }
        public Task<IEnumerable<ImageHomeStayRentals>> GetAllAsync()
        {
            return _imageHomeStayTypesDAO.GetAllAsync();
        }

        public Task<IEnumerable<ImageHomeStayRentals>> GetAllByImageIdAsync(int imageId)
        {
            return _imageHomeStayTypesDAO.GetAllByImageIdAsync(imageId);
        }


        public Task<ImageHomeStayRentals> GetImageHomeStayTypesByIdAsync(int id)
        {
            return _imageHomeStayTypesDAO.GetImageHomeStayTypesByIdAsync(id);
        }

        public async Task<ImageHomeStayRentals> AddImageAsync(ImageHomeStayRentals image)
        {
            return await _imageHomeStayTypesDAO.AddAsync(image);
        }
    }
}
