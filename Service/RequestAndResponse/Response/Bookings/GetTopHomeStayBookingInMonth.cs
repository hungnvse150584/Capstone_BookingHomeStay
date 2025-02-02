using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetTopHomeStayBookingInMonth
    {
        public List<(string homeStayName, int QuantityOfBooking)> topHomeStays { get; set; }
    }
}
