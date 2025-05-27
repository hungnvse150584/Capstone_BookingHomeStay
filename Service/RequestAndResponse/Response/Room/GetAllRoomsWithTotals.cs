using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Room
{
    public class GetAllRoomsWithTotals
    {
        public IEnumerable<GetAllRooms> Rooms { get; set; }
        public int TotalRooms { get; set; }
        public int TotalBathRooms { get; set; }
        public int TotalWifis { get; set; }
    }
}
