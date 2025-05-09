using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetStaticBookingsForHomeStay
    {
        public int bookingsReturnOrCancell { get; set; }
        public int bookings { get; set; }
        public int bookingsComplete { get; set; }
        public int bookingsCancell { get; set; }
        public int bookingsReturnRefund { get; set; }
        public int bookingsReport { get; set; }
        public int bookingConfirmed { get; set; }
    }
}
