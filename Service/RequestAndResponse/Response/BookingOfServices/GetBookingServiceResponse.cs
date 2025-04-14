using BusinessObject.Model;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingOfServices
{
    public class GetBookingServiceResponse
    {
        public int BookingServicesID { get; set; }

        public int? BookingID { get; set; }

        public DateTime BookingServicesDate { get; set; }

        public double Total { get; set; }

        public BookingServicesStatus Status { get; set; }

        public PaymentServicesMethod PaymentServicesMethod { get; set; }

        public PaymentServicesStatus PaymentServiceStatus { get; set; }

        public ICollection<SingleServiceDetail> BookingServicesDetails { get; set; }
    }
}
