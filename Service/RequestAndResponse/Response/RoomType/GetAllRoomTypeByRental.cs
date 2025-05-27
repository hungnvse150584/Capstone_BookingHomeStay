using Service.RequestAndResponse.Request.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.RoomType
{
    public class GetAllRoomTypeByRental
    {
       
        public int RoomTypesID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
      
        public bool? Status { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        public ICollection<ImageRoomTypeResponse>? ImageRoomTypes { get; set; }

        public ICollection<PricingForHomeStayRental>? Pricings { get; set; }
    }
}
