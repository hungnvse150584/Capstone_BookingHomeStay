using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Pricing
{
    public class CreatePricingRequest
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public double RentPrice { get; set; }
        public double Percentage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public DayType DayType { get; set; }

        public int? HomeStayRentalID { get; set; }

        public int? RoomTypesID { get; set; }
    }
}
