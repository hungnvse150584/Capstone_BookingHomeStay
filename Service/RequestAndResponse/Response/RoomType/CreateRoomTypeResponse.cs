using Service.RequestAndResponse.Request.Pricing;
using System;
using System.Collections.Generic;

namespace Service.RequestAndResponse.Response.RoomType
{
    public class CreateRoomTypeResponse
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int numberBedRoom { get; set; }
        public int numberBathRoom { get; set; }
        public int numberWifi { get; set; }
        public bool? Status { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        public ICollection<ImageRoomTypeResponse>? ImageRoomTypes { get; set; }

        public ICollection<PricingForHomeStayRental>? Pricings { get; set; } // Thêm Pricings
        //public ICollection<RoomResponse>? Rooms { get; set; } // Thêm Rooms
    }

    // DTO cho Pricing trong response
   
    // DTO cho Room trong response
    //public class RoomResponse
    //{
        
    //    public string roomNumber { get; set; }

    //    public bool isUsed { get; set; }

    //    public bool isActive { get; set; }

    //    public int? RoomTypesID { get; set; }
    //}
}