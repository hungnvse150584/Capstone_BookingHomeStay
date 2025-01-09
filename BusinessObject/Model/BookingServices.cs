using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class BookingServices
    {
        [Key]
        public int BookingServicesID { get; set; }

        [ForeignKey("BookingID")]
        public int? BookingID { get; set; } 
        public Booking? Booking { get; set; } 

        public DateTime BookingServicesDate { get; set; }

        public double Total { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public string? transactionID { get; set; }
        public Transaction? Transaction { get; set; }


        [EnumDataType(typeof(BookingServicesStatus))]
        public BookingServicesStatus Status { get; set; }

        [EnumDataType(typeof(PaymentServicesMethod))]
        public PaymentServicesMethod PaymentServicesMethod { get; set; }

        public ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }

    public enum PaymentServicesMethod
    {
        Cod = 0,
        PayOS = 1
    }

    public enum BookingServicesStatus
    {
        ToPay = 0,
        ToConfirm = 1,
        Completed = 3,
        Cancelled = 4,
        ReturnRefund = 5,
        RequestReturn = 6
    }
}
