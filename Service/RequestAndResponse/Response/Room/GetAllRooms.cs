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

namespace Service.RequestAndResponse.Response.Room
{
    public class GetAllRooms
    {
        public int RoomID { get; set; }

        public string roomNumber { get; set; }

        public bool isUsed { get; set; }

        public bool isActive { get; set; }

        public int? RoomTypesID { get; set; }
        //public GetAllRoomType? RoomTypes { get; set; }

        //public ICollection<GetBookingDetails> BookingDetails { get; set; }
    }
}
