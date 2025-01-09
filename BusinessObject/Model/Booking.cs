using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public DateTime BookingDate { get; set; }

        public int numberOfChildren { get; set; }

        public int numberOfAdults { get; set; }


        [EnumDataType(typeof(BookingStatus))]
        public BookingStatus Status { get; set; }

        public double Total { get; set; }

        public int? ReportID { get; set; }
        public Report? Report { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }    
        public Account Account { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }

        public ICollection<BookingServices> BookingServices { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }

    public enum BookingStatus
    {
        ToPay = 0,
        ToConfirm = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        ReturnRefund = 5,
        RequestReturn = 6,
        NoShow = 7

    }

    public enum PaymentMethod
    {
        Cod = 0,
        PayOS = 1
    }

