using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.ImageRating
{
    public class UpdateImageRatingRequest
    {
   
        public IFormFile File { get; set; }
    }
}
