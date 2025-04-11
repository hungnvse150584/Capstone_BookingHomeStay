using Service.RequestAndResponse.Request.BookingServiceDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingServices
{
    public class CreateBookingServices
    {
        public int BookingID { get; set; }
        public DateTime BookingServicesDate { get; set; }
        public string AccountID { get; set; }
        public int HomeStayID { get; set; }

        public ICollection<BookingServiceDetailRequest> BookingServicesDetails { get; set; }
    }
}
