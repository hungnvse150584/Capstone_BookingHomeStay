using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Room
{
    public class CreateRoomRequest
    {
        public string roomNumber { get; set; }

        public bool isUsed { get; set; } = true;

        public bool isActive { get; set; } = true;

        public int? RoomTypesID { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
