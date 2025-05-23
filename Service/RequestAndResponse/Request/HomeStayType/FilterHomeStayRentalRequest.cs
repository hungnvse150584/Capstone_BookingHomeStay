﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStayType
{
    public class FilterHomeStayRentalRequest
    {

        [Required(ErrorMessage = "HomeStayID is required.")]
        public int HomeStayID { get; set; }
        public bool? RentWhole { get; set; }
        public DateTime? CheckInDate { get; set; }

       
        public DateTime? CheckOutDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of adults cannot be negative.")]
        public int? NumberOfAdults { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of children cannot be negative.")]
        public int? NumberOfChildren { get; set; }
        public decimal? MinPrice { get; set; } 
        public decimal? MaxPrice { get; set; } 
        public int? Rating { get; set; }

    }
}
