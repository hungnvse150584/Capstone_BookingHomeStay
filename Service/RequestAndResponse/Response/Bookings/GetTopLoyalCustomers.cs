using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetTopLoyalCustomers
    {
        public string accountID { get; set; }
        public string CustomerName { get; set; }
        public int totalBookings { get; set; }
    }
}
