using BusinessObject.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<List<Notification>> GetNotificationsByAccountIdAsync(string accountId);
        Task<Notification> MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(string accountId);
    }
}