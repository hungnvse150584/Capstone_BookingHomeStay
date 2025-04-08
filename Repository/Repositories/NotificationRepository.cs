using BusinessObject.Model;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repository.BaseRepository;
using Repository.IRepositories;
using System.Collections.Generic;
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

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            return await _notificationDao.AddAsync(notification);
        }

        public async Task<List<Notification>> GetNotificationsByAccountIdAsync(string accountId)
        {
            return await _notificationDao.GetNotificationsByAccountIdAsync(accountId);
        }

        public async Task<Notification> MarkNotificationAsReadAsync(int notificationId)
        {
            return await _notificationDao.MarkNotificationAsReadAsync(notificationId);
        }

        public async Task MarkAllNotificationsAsReadAsync(string accountId)
        {
            await _notificationDao.MarkAllNotificationsAsReadAsync(accountId);
        }
        public async Task<Notification> GetNotificationByDetailsAsync(string accountId, int? bookingId, string title, string message)
        {
            return await _notificationDao.GetNotificationByDetailsAsync(accountId, bookingId, title, message);
        }
    }
}