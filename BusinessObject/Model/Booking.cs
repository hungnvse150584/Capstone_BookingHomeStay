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
    
        public DateTime ExpiredTime { get; set; }

        public int numberOfChildren { get; set; }

        public int numberOfAdults { get; set; }


        [EnumDataType(typeof(BookingStatus))]
        public BookingStatus Status { get; set; }


        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus paymentStatus { get; set; }

        public double TotalRentPrice { get; set; }

        public double Total { get; set; }

        public double bookingDeposit { get; set; }

        public double remainingBalance { get; set; }

        public int? ReportID { get; set; }
        public Report? Report { get; set; }

        public int? RatingID { get; set; }
        public Rating Rating { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }    
        public Account Account { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<BookingDetail> BookingDetails { get; set; }

        public ICollection<BookingServices> BookingServices { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }

    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        NoShow = 5,
        RequestRefund = 6
    }

public enum PaymentStatus
    {
        Pending = 0,       
        Deposited = 1,      
        FullyPaid = 2,     
        Refunded = 3
    }

    

    public enum PaymentMethod
    {
        Cod = 0,
        VnPay = 1
    }

