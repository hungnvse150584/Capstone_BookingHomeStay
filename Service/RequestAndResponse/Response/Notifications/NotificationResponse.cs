using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Notifications
{
    public class NotificationResponse
    {
        public int NotificationID { get; set; }
        public string AccountID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreateAt { get; set; }
        public int? BookingID { get; set; }
        public int? BookingServicesID { get; set; }
        public bool IsRead { get; set; }
        public NotificationType TypeNotify { get; set; }
    }
}
