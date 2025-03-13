using Service.RequestAndResponse.Request.BookingDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Booking
{
    public class UpdateBookingRequest
    {
        public int numberOfChildren { get; set; }

        public int numberOfAdults { get; set; }

        public ICollection<UpdateBookingDetailRequest> BookingDetails { get; set; }
    }
}
