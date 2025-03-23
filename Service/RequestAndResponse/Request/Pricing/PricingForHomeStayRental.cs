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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int DayType { get; set; }
        public string Description { get; set; }
    }
}
