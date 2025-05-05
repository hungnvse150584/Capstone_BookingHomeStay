using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Rating
{
    public class CreateRatingRequest
    {
     
        public double? CleaningRate { get; set; }
        public double? ServiceRate { get; set; }
        public double? FacilityRate { get; set; }
        public string? Content { get; set; }
        public string AccountID { get; set; }
        public int HomeStayID { get; set; }
        public int BookingID { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
