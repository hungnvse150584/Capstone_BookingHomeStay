using BusinessObject.Model;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingOfServices
{
    public class GetBookingServiceByHomeStay
    {
        public int BookingServicesID { get; set; }

        public int? BookingID { get; set; }

        public DateTime BookingServicesDate { get; set; }

        public double Total { get; set; }

        public string AccountID { get; set; }
        public GetAccountUser Account { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public BookingServicesStatus Status { get; set; }

        public PaymentServicesMethod PaymentServicesMethod { get; set; }

        public ICollection<GetSingleDetailOfService> BookingServicesDetails { get; set; }
    }
}
