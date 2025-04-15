using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetTotalBookingsTotalBookingsAmountForHomeStay
    {
        public object span { get; set; }
        public int totalBookings { get; set; }
        public double totalBookingsAmount { get; set; }
    }
}
