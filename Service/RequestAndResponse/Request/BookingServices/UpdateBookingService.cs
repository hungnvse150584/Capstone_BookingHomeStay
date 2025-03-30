using Service.RequestAndResponse.Request.BookingServiceDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingServices
{
    public class UpdateBookingService
    {
        public int BookingID { get; set; }

        public string AccountID { get; set; }

        public ICollection<UpdateBookingServiceDetailRequest> BookingServicesDetails { get; set; }
    }
}
