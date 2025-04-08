using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Pricing
{
    public class GetPricing
    {
        public int PricingID { get; set; }
        public int HomeStayRentalID { get; set; }
        public int UnitPrice { get; set; }
        public int RentPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDefault { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public DataType DayType { get; set; }
        public string Description { get; set; }
    }
}
