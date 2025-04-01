using Service.RequestAndResponse.Request.Pricing;
using System.Collections.Generic;

namespace Service.RequestAndResponse.Response.RoomType
{
    public class GetAllRoomTypeForFilter
    {
        public int RoomTypesID { get; set; }
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
        public ICollection<PricingForHomeStayRental>? Pricings { get; set; }
        public ICollection<GetRoomResponse> Rooms { get; set; } = new List<GetRoomResponse>();
        public int AvailableRoomsCount { get; set; }
    }

    public class GetRoomResponse
    {
        public int RoomID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int RoomTypesID { get; set; }
    }
}