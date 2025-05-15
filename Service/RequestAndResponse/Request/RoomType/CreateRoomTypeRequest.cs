using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.RoomType
{
    public class CreateRoomTypeRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int numberBed { get; set; }
        public int numberBathRoom { get; set; }
        public int numberWifi { get; set; }
        public bool Status { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        //public List<IFormFile> Images { get; set; }
        public string? PricingJson { get; set; } 
        public List<PricingForHomeStayRental> Pricing { get; set; } 
        //public string? RoomsJson { get; set; }
        //public List<CreateRoomRequest> Rooms { get; set; } 
       

        // DTO cho Room
        
    }
}
