using BusinessObject.Model;
using Repository.IRepositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            // Kiểm tra xem thông báo với cùng AccountID, BookingID, Title, và Message đã tồn tại chưa
            var existingNotification = await _notificationRepository.GetNotificationByDetailsAsync(
                notification.AccountID,
                notification.BookingID,
                notification.Title,
                notification.Message
            );

            if (existingNotification != null)
            {
                // Nếu đã tồn tại, không tạo mới, trả về thông báo hiện có
                return existingNotification;
            }

            notification.CreateAt = DateTime.UtcNow;
            notification.IsRead = false;

            return await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task<List<Notification>> GetNotificationsByAccountIdAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            return await _notificationRepository.GetNotificationsByAccountIdAsync(accountId);
        }

        public async Task<Notification> MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.MarkNotificationAsReadAsync(notificationId);
            if (notification == null)
            {
                throw new Exception("Notification not found.");
            }
            return notification;
        }

        public async Task MarkAllNotificationsAsReadAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            await _notificationRepository.MarkAllNotificationsAsReadAsync(accountId);
        }
    }
}