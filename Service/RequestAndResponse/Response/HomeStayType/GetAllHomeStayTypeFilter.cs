using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;

namespace Service.RequestAndResponse.Response.HomeStayType
{
    public class GetAllHomeStayTypeFilter
    {
        public int HomeStayRentalID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int numberBedRoom { get; set; }
        public int numberBathRoom { get; set; }
        public int numberKitchen { get; set; }
        public int numberWifi { get; set; }
        public bool Status { get; set; } = true;
        public bool RentWhole { get; set; }
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        public int? HomeStayID { get; set; }
        public int TotalAvailableRooms { get; set; }
        public IEnumerable<GetAllImageHomeStayType> ImageHomeStayRentals { get; set; }
        public ICollection<GetAllRoomTypeForFilter> RoomTypes { get; set; }
        public IEnumerable<GetBookingDetailFilter> BookingDetails { get; set; } // Sửa kiểu thành GetBookingDetailFilter
        public ICollection<GetAllPricing> Pricing { get; set; }
    }
}