using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Pricing
{
    public class PricingForHomeStayRental
    {
        public int UnitPrice { get; set; }
        public int RentPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Percentage { get; set; } 
        public bool IsDefault { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public DayType DayType { get; set; }
        public string Description { get; set; }
       
    }
}
