﻿using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.RoomType;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.ImageRooms;

namespace Service.RequestAndResponse.Response.Room
{
    public class GetAllRooms
    {
        public int RoomID { get; set; }

        public string roomNumber { get; set; }

        //public bool isUsed { get; set; }

        public bool isActive { get; set; }
        public int numberBed { get; set; }


        public int numberBathRoom { get; set; }

        public string? homestayName { get; set; }
        public int numberWifi { get; set; }
        public int? RoomTypesID { get; set; }
        public string RoomTypeName { get; set; } // Chỉ lấy tên của RoomType
        public decimal? RentPrice { get; set; }
        public string? HomeStayRentalName {  get; set; }
        public int? HomeStayRentalID { get; set; }
        public List<ImageRoomResponse>? ImageRooms { get; set; }

        //public GetAllRoomType? RoomTypes { get; set; }

        //public ICollection<GetBookingDetails> BookingDetails { get; set; }
    }
   
}
