using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingDetails
{
    public class GetBookingDetailResponse
    {
        public int BookingDetailID { get; set; }

        public double rentPrice { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public double TotalAmount { get; set; }

        public int? HomeStayTypesID { get; set; }
        public GetSimpleHomeStayType? HomeStayTypes { get; set; }

        public GetRoomsResponse? Rooms { get; set; }
    }
}
