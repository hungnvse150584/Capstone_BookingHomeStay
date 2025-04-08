using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class NotificationDAO : BaseDAO<Notification>
    {
        private readonly GreenRoamContext _context;

        public NotificationDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        // Lấy danh sách thông báo theo AccountID
        public async Task<List<Notification>> GetNotificationsByAccountIdAsync(string accountId)
        {
            return await _context.Notifications
                .Where(n => n.AccountID == accountId)
                .Include(n => n.Account)
                .Include(n => n.Booking)
                .Include(n => n.BookingService)
                .OrderByDescending(n => n.CreateAt)
                .ToListAsync();
        }

        // Đánh dấu một thông báo là đã đọc
        public async Task<Notification> MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await UpdateAsync(notification);
            }
            return notification;
        }

        // Đánh dấu tất cả thông báo của một tài khoản là đã đọc
        public async Task MarkAllNotificationsAsReadAsync(string accountId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.AccountID == accountId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await UpdateRange(notifications);
        }
        public async Task<Notification> GetNotificationByDetailsAsync(string accountId, int? bookingId, string title, string message)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.AccountID == accountId &&
                    n.BookingID == bookingId &&
                    n.Title == title &&
                    n.Message == message);
        }
    }
}