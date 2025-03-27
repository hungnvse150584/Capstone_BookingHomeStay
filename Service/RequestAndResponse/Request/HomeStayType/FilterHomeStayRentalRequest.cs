using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStayType
{
    public class FilterHomeStayRentalRequest
    {
    
        public DateTime CheckInDate { get; set; }

       
        public DateTime CheckOutDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of adults cannot be negative.")]
        public int NumberOfAdults { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of children cannot be negative.")]
        public int NumberOfChildren { get; set; }

        public bool? RentWhole { get; set; } 
    }
}
