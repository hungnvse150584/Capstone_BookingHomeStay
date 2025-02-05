using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ImageHomeStayTypesRepository : BaseRepository<ImageHomeStayTypes>, IImageHomeStayTypesRepository
    {
        private readonly ImageHomeStayTypesDAO _imageHomeStayTypesDAO;
        public ImageHomeStayTypesRepository(ImageHomeStayTypesDAO imageHomeStayTypesDAO) : base(imageHomeStayTypesDAO)
        {
            _imageHomeStayTypesDAO = imageHomeStayTypesDAO;
        }
        public Task<ImageHomeStayTypes> AddAsync(ImageHomeStayTypes entity)
        {
            return _imageHomeStayTypesDAO.AddAsync(entity);
        }

        public Task<ImageHomeStayTypes> UpdateAsync(ImageHomeStayTypes entity)
        {
            return _imageHomeStayTypesDAO.UpdateAsync(entity);
        }

        public Task<ImageHomeStayTypes> DeleteAsync(ImageHomeStayTypes entity)
        {
            return _imageHomeStayTypesDAO.DeleteAsync(entity);
        }
        public Task<IEnumerable<ImageHomeStayTypes>> GetAllAsync()
        {
            return _imageHomeStayTypesDAO.GetAllAsync();
        }

        public Task<IEnumerable<ImageHomeStayTypes>> GetAllByImageIdAsync(int imageId)
        {
            return _imageHomeStayTypesDAO.GetAllByImageIdAsync(imageId);
        }


        public Task<ImageHomeStayTypes> GetImageHomeStayTypesByIdAsync(int id)
        {
            return _imageHomeStayTypesDAO.GetImageHomeStayTypesByIdAsync(id);
        }
    }
}
