using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingServiceDetails
{
    public class UpdateBookingServiceDetailRequest
    {
        public int? ServiceDetailID { get; set; }

        public int Quantity { get; set; }

        public int? dayRent { get; set; }

        public int? RentHour { get; set; }

        public int ServicesID { get; set; }
    }
}
