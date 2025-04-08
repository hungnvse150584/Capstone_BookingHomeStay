using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Notifications
{
    public class CreateNotificationRequest
    {
        public string AccountID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? BookingID { get; set; }
        public int? BookingServicesID { get; set; }
        public NotificationType TypeNotify { get; set; }
    }

    public class MarkNotificationAsReadRequest
    {
        public int NotificationID { get; set; }
    }

    public class MarkAllNotificationsAsReadRequest
    {
        public string AccountID { get; set; }
    }
}
