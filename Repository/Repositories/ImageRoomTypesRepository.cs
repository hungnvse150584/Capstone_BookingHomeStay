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
    public class ImageRoomRepository : BaseRepository<ImageRoom>, IImageRoomRepository
    {
        private readonly ImageRoomDAO _roomDao;

        public ImageRoomRepository(ImageRoomDAO roomDao) : base(roomDao)
        {
            _roomDao = roomDao;
        }

        public async Task AddImageAsync(ImageRoom image)
        {
             await _roomDao.AddAsync(image);
        }

        public async Task SaveChangesAsync()
        {
            await _roomDao.SaveChangesAsync();
        }
    }
}
