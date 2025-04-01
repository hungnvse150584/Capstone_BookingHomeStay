using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingDetails
{
    public class GetSimpleBookingDetail
    {
        public int BookingDetailID { get; set; }

        public double rentPrice { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public double TotalAmount { get; set; }

        public int? HomeStayRentalID { get; set; }
        public GetSimpleHomeStayType? HomeStayRentals { get; set; }

        public int? RoomID { get; set; }
        public GetAllRooms Rooms { get; set; }
    }
}
