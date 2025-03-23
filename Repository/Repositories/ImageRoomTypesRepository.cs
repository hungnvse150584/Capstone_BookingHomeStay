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
    public class ImageRoomTypesRepository : BaseRepository<ImageRoomTypes>, IImageRoomTypesRepository
    {
        private readonly ImageRoomTypeDAO _roomTypeDao;

        public ImageRoomTypesRepository(ImageRoomTypeDAO roomTypeDao) : base(roomTypeDao)
        {
            _roomTypeDao = roomTypeDao;
        }

        public async Task AddImageAsync(ImageRoomTypes image)
        {
             await _roomTypeDao.AddAsync(image);
        }

        public async Task SaveChangesAsync()
        {
            await _roomTypeDao.SaveChangesAsync();
        }
    }
}
