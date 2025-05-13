using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Pricing
{
    public class PricingResponse
    {
        public int PricingID { get; set; }

        public string Description { get; set; }



        public double RentPrice { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public DayType DayType { get; set; }
        public int? HomeStayRentalID { get; set; }

    }
}
