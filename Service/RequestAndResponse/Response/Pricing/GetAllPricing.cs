using BusinessObject.Model;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Pricing
{
    public class GetAllPricing
    {
        
        public int PricingID { get; set; }

        public string Description { get; set; }

        public double UnitPrice { get; set; }

        public double RentPrice { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public DayType DayType { get; set; }

        public int? HomeStayRentalID { get; set; }
        public GetAllHomeStayType? HomeStayRentals { get; set; }

        public int? RoomTypesID { get; set; }
        public GetAllRoomType? RoomTypes { get; set; }
    }
}
