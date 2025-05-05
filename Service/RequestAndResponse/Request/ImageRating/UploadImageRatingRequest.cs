using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.ImageRating
{
    public class UploadImageRatingRequest
    {
    
        public IFormFile File { get; set; }

   
        public int RatingID { get; set; }
    }
}
