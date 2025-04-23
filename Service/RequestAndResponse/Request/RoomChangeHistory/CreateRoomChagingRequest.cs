using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.RoomChangeHistory
{
    public class CreateRoomChagingRequest
    {
        public int BookingDetailID { get; set; }
        public int? OldRoomID { get; set; }
        public int? NewRoomID { get; set; }
        public DateTime UsagedDate { get; set; }
        public DateTime ChangedDate { get; set; }
        public string AccountID { get; set; }
    }
}
