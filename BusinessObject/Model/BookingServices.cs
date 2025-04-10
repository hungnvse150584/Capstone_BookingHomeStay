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

            public double bookingServiceDeposit { get; set; }

            public double remainingBalance { get; set; }

            [ForeignKey("AccountID")]
            public string AccountID { get; set; }
            public Account Account { get; set; }

            public int? HomeStayID { get; set; }
            [ForeignKey("HomeStayID")]
            public HomeStay? HomeStay { get; set; }


            [EnumDataType(typeof(BookingServicesStatus))]
            public BookingServicesStatus Status { get; set; }


            [EnumDataType(typeof(PaymentServicesMethod))]
            public PaymentServicesMethod PaymentServicesMethod { get; set; }


            [EnumDataType(typeof(PaymentServicesStatus))]
            public PaymentServicesStatus PaymentServiceStatus { get; set; }

            public ICollection<Transaction> Transactions { get; set; }

            public ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }

            public ICollection<Notification> Notifications { get; set; }
        }

        public enum PaymentServicesMethod
        {
            Cod = 0,
            VnPay = 1
        }

        public enum PaymentServicesStatus
        {
            Pending = 0,
            Deposited = 1,
            FullyPaid = 2,
            Refunded = 3
        }

        public enum BookingServicesStatus
        {
            Pending = 0,
            Confirmed = 1,
            Completed = 3,
            Cancelled = 4
        }
    }
