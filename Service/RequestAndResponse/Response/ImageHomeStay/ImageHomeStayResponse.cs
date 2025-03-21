using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Service.RequestAndResponse.Response.ImageHomeStay
{
    public class ImageHomeStayResponse
    {
        [Key]
        public int ImageHomeStayID { get; set; }

        [Required]
        public string? Image { get; set; }


       
    }
}
