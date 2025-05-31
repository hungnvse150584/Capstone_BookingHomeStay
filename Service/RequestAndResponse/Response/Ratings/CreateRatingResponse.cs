using Microsoft.AspNetCore.Http;
using Service.RequestAndResponse.Response.ImageRating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Ratings
{
    public class CreateRatingResponse
    {
        public int RatingID { get; set; }
        public double SumRate { get; set; }
        public double CleaningRate { get; set; }
        public double ServiceRate { get; set; }
        public double FacilityRate { get; set; }
        public string Content { get; set; }
        public string AccountID { get; set; }
        public string Username { get; set; }
        public int HomeStayID { get; set; }
        public int BookingID { get; set; }
        public string? homestayName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<ImageRatingResponse> ImageRatings { get; set; }
    }

}