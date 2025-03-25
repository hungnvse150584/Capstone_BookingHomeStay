using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStay
{
    public class UpdateHomeStayImagesBodyRequest
    {

        public int? ImageHomeStayID { get; set; } 
        public List<IFormFile> Images { get; set; }
    
    }
}
