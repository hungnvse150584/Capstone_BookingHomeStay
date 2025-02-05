using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.ImageService
{
    public class AddImageServicesRequest
    {
        [Key]
        public int ImageServicesID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? ServicesID { get; set; }
    }
}
