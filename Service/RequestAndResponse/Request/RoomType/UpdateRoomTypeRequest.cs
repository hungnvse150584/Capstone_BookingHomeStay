using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.RoomType
{
    public class UpdateRoomTypeRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int numberBed { get; set; }
        public int numberBathRoom { get; set; }
        public int numberWifi { get; set; }
        public bool Status { get; set; } = true;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
    }
}
