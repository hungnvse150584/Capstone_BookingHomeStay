using System;
using System.Collections.Generic;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.Room;
using Service.RequestAndResponse.Response.RoomType;

namespace Service.RequestAndResponse.Response.HomeStayType
{
    public class GetHomeStayRentalDetailResponse
    {
        public int HomeStayRentalID { get; set; }
        public string Name { get; set; }
        public string HomeStayName { get; set; }

        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public int numberBedRoom { get; set; }

        public int numberBathRoom { get; set; }

        public int numberKitchen { get; set; }

        public int numberWifi { get; set; }

        public bool Status { get; set; }

        public bool RentWhole { get; set; }

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }

        public int? HomeStayID { get; set; }
        public IEnumerable<GetAllImageHomeStayType> ImageHomeStayRentals { get; set; }
        public ICollection<RoomTypeDetailResponse> RoomTypes { get; set; }


        public IEnumerable<GetBookingDetails> BookingDetails { get; set; }

        public ICollection<PricingResponse> Pricing { get; set; }
    }

    public class RoomTypeDetailResponse
    {
        public int RoomTypesID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int numberBed { get; set; }
        public int numberBathRoom { get; set; }
        public int numberWifi { get; set; }
        public bool Status { get; set; }
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        public IEnumerable<ImageRoomTypeResponse> ImageRoomTypes { get; set; }
        public IEnumerable<PricingResponse> Pricings { get; set; }
        //public IEnumerable<RoomResponse> Rooms { get; set; }
    }
        
}