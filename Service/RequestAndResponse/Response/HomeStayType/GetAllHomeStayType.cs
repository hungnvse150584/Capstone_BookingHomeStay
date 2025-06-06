﻿using BusinessObject.Model;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.HomeStayType
{
    public class GetAllHomeStayType
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

        public bool Status { get; set; }

        public bool RentWhole { get; set; }

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }

        public int? HomeStayID { get; set; }
        public int TotalAvailableRooms { get; set; }
        public string? homestayName {  get; set; }
        public IEnumerable<GetAllImageHomeStayType> ImageHomeStayRentals { get; set; }
        public ICollection<GetAllRoomType> RoomTypes { get; set; }

       
        public IEnumerable<GetSimpleBookingDetail> BookingDetails { get; set; }

        public ICollection<PricingResponse> Pricing { get; set; }
    }
}
