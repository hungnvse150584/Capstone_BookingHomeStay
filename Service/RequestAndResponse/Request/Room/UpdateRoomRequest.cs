using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Room
{
    public class UpdateRoomRequest
    {
        public string roomNumber { get; set; }

  

        public bool isActive { get; set; }
        public int numberBed { get; set; }


        public int numberBathRoom { get; set; }


        public int numberWifi { get; set; }

        public int? RoomTypesID { get; set; }
  
    }
}
