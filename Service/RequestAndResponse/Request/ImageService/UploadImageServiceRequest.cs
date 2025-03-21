using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.ImageService
{
    public class UploadImageServiceRequest
    {
        [Required]
        public IFormFile File { get; set; }

        public int? ServiceID { get; set; }

    }
}
