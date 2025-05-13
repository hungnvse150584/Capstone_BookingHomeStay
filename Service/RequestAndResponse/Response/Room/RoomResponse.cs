using BusinessObject.Model;
using Service.RequestAndResponse.Response.ImageRooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Room
{
    public class RoomResponse
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public bool IsUsed { get; set; }
        public bool IsActive { get; set; }
        public RoomTypeResponse RoomType { get; set; }
        public decimal? RentPrice { get; set; } // Giá thuê, có thể lấy từ HomeStayRental hoặc BookingDetails
        public ICollection<ImageRoomResponse> ImageRooms { get; set; }
    }

    public class RoomTypeResponse
    {
        public int RoomTypesID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberBedRoom { get; set; }
        public int NumberBathRoom { get; set; }
        public int NumberWifi { get; set; }
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
    }
}
