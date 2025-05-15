using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStays
{
    public class CreateHomeStayWithRentalsAndPricingResponse
    {
        public HomeStayResponse HomeStay { get; set; } // Đã có ánh xạ từ HomeStay -> HomeStayResponse

        public List<GetAllHomeStayType> Rentals { get; set; } // Sửa thành List<GetAllHomeStayType>

        public List<PricingResponse> Pricings { get; set; }
        public List<CreateRoomTypeResponse> RoomTypes { get; set; }
    }
}
