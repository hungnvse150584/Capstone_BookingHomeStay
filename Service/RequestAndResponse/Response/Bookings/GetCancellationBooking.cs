using BusinessObject.Model;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.BookingOfServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetCancellationBooking
    {
        public int BookingID { get; set; }

        public string BookingCode { get; set; }

        public DateTime BookingDate { get; set; }

        public int numberOfChildren { get; set; }

        public int numberOfAdults { get; set; }

        public BookingStatus Status { get; set; }

        public double Total { get; set; }

        public double CancelFee { get; set; }

        public int? ReportID { get; set; }
        public Report? Report { get; set; }

        public string AccountID { get; set; }
        public GetAccountUser Account { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<GetBookingDetails> BookingDetails { get; set; }

        public ICollection<GetAllBookingServices> BookingServices { get; set; }
    }
}
