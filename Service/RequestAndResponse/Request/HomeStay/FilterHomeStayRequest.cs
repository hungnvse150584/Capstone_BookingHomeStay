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
        [Required(ErrorMessage = "Check-in date is required.")]
        public DateTime CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-out date is required.")]
        public DateTime CheckOutDate { get; set; }

        [Required(ErrorMessage = "Number of adults is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of adults must be at least 1.")]
        public int NumberOfAdults { get; set; }

        [Required(ErrorMessage = "Number of children is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of children cannot be negative.")]
        public int NumberOfChildren { get; set; }
    }
}
