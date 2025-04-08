using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface INotificationRepository
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByDetailsAsync(string accountId, int? bookingId, string title, string message);
        Task<List<Notification>> GetNotificationsByAccountIdAsync(string accountId);
        Task<Notification> MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(string accountId);
    }
}
