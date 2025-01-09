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
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        private readonly NotificationDAO _notificationDao;

        public NotificationRepository(NotificationDAO notificationDao) : base(notificationDao)
        {
            _notificationDao = notificationDao;
        }

        public Task<Notification> AddAsync(Notification entity)
        {
            return _notificationDao.AddAsync(entity);
        }

        public Task<Notification> UpdateAsync(Notification entity)
        {
            return _notificationDao.UpdateAsync(entity);
        }

        public Task<Notification> DeleteAsync(Notification entity)
        {
            return _notificationDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<Notification>> GetAllAsync()
        {
            return _notificationDao.GetAllAsync();
        }
    }
}
