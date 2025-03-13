using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime CreateAt { get; set; }

        public int? BookingID { get; set; }
        [ForeignKey("BookingID")]
        public Booking? Booking { get; set; }

        public int? BookingServicesID { get; set; }
        [ForeignKey("BookingServicesID")]
        public BookingServices? BookingService { get; set; }

        public bool IsRead { get; set; }

        public NotificationType TypeNotify { get; set; }

    }

    public enum NotificationType
    {
        BookingSuccessfully = 0,
        Reminder = 1,
        AdminAnnouncement = 2
    }

