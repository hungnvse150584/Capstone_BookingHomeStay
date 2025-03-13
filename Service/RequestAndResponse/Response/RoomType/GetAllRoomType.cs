using BusinessObject.Model;
using Service.RequestAndResponse.Response.HomeStayType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.RoomType
{
    public class GetAllRoomType
    {
        public int RoomTypesID { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public double UnitPrice { get; set; }

        public double RentPrice { get; set; }

        public int numberBedRoom { get; set; }

        public int numberBathRoom { get; set; }

        public int numberWifi { get; set; }

        public bool Status { get; set; }

        public int MaxAdults { get; set; }

        public int MaxChildren { get; set; }

        public int MaxPeople { get; set; }

        public int? HomeStayRentalID { get; set; }

        public GetAllHomeStayType HomeStayRental { get; set; }

        /*public ICollection<GetAllRoomAvaibility> RoomAvaibilities { get; set; }*/
    }
}
