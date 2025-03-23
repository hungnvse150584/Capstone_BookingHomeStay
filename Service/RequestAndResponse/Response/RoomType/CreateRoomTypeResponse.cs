using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.RoomType
{
    public class CreateRoomTypeResponse
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int numberBedRoom { get; set; }
        public int numberBathRoom { get; set; }
        public int numberWifi { get; set; }
        public bool? Status { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public int MaxPeople { get; set; }
        public ICollection<ImageRoomTypes>? ImageRoomTypes { get; set; }
    }
}
