using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.Accounts;


namespace Service.RequestAndResponse.Response.BookingOfServices
{
    public class GetAllBookingServices
    {
        public int BookingServicesID { get; set; }

        public int? BookingID { get; set; }
        public Booking? Booking { get; set; }

        public DateTime BookingServicesDate { get; set; }

        public double Total { get; set; }

        public string AccountID { get; set; }
        public GetAccountUser Account { get; set; }

        public string? transactionID { get; set; }
        public Transaction? Transaction { get; set; }

        public BookingServicesStatus Status { get; set; }

        public PaymentServicesMethod PaymentServicesMethod { get; set; }

        public ICollection<BookingServicesDetail> BookingServicesDetails { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
