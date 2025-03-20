using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Service.RequestAndResponse.Request.ImageHomeStay
{
    public class UploadImageRequest
    {
        [Required]
        public IFormFile File { get; set; }

        public int? HomeStayID { get; set; }
     
    }
}
