using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStay
{
    public class FilterHomeStayRequest
    {
        
        public DateTime CheckInDate { get; set; }

     
        public DateTime CheckOutDate { get; set; }

        
        [Range(0, int.MaxValue, ErrorMessage = "Number of adults cannot be negative.")]
        public int NumberOfAdults { get; set; }

        
        [Range(0, int.MaxValue, ErrorMessage = "Number of children cannot be negative.")]
        public int NumberOfChildren { get; set; }
        [Required]
        public double Latitude { get; set; } 

        [Required]
        public double Longitude { get; set; } 

        [Range(0, double.MaxValue, ErrorMessage = "Max distance must be non-negative.")]
        public double MaxDistance { get; set; } = 10;
        public int? Rating { get; set; } 
        public decimal? MinPrice { get; set; } 
        public decimal? MaxPrice { get; set; }
    }
}
