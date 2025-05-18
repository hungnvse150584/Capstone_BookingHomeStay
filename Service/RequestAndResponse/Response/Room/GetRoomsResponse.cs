using BusinessObject.Model;
using Service.RequestAndResponse.Response.ImageRooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Room
{
    public class GetRoomsResponse
    {
        /*public int RoomID { get; set; }

        public string roomNumber { get; set; }

        public bool isUsed { get; set; }

        public bool isActive { get; set; }

        public int? RoomTypesID { get; set; }

        public ICollection<ImageRoomResponse> ImageRooms { get; set; }*/

        public int RoomID { get; set; }

        public string roomNumber { get; set; }

        public bool isActive { get; set; }

        public int? RoomTypesID { get; set; }
        public string RoomTypeName { get; set; } // Chỉ lấy tên của RoomType
        public decimal? RentPrice { get; set; }
        public string? HomeStayRentalName { get; set; }
        public List<ImageRoomResponse>? ImageRooms { get; set; }
    }
}
