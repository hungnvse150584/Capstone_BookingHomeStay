using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingDetail
{
    public class UpdateBookingDetailRequest
    {
        public int? BookingDetailID { get; set; }
        public int homeStayTypeID { get; set; }
        public int? roomTypeID { get; set; }

        public int Quantity { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
    }
}
