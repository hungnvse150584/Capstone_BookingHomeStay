using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Pricing
{
    public class PricingForHomeStayRental
    {
        [Required]
        public double RentPrice { get; set; }
    }
}
