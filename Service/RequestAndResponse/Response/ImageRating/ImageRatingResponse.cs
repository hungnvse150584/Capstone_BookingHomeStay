using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.ImageRating
{
    public class ImageRatingResponse
    {
        public int ImageRatingID { get; set; }
        public string Image { get; set; }
        public int RatingID { get; set; }
    }
}
