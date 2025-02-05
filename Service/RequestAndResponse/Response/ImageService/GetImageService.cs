using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.ImageService
{
    public class GetImageService
    {
        [Key]
        public int ImageServicesID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? ServicesID { get; set; }
        //[ForeignKey("ServicesID")]
        //public Services? Services { get; set; }
    }
}
