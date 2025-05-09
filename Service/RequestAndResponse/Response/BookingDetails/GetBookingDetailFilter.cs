using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.HomeStayType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingDetails
{
    public class GetBookingDetailFilter
    {
        public int BookingDetailID { get; set; }

        public double rentPrice { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public double TotalAmount { get; set; }

        public int? HomeStayRentalID { get; set; }
        public int? RoomID { get; set; }
        public GetAllHomeStayType? HomeStayTypes { get; set; }

        public int? BookingID { get; set; }
        public GetAllBookings? Booking { get; set; }
       
       
    }
}
