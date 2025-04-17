using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetCurrentWeekRevenueForHomeStay
    {
        public string Date { get; set; }
        public double totalBookingsAmount { get; set; }
    }
}
