using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Room
{
    public class GetRoomsResponse
    {
        public int RoomID { get; set; }

        public string roomNumber { get; set; }

        public bool isUsed { get; set; }

        public bool isActive { get; set; }

        public int? RoomTypesID { get; set; }
    }
}
