using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Service.RequestAndResponse.Request.HomeStay
{
    public class CreateHomeStayRequest
    {
        [Required(ErrorMessage = "HomeStay must have name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "HomeStay must have Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "HomeStay must have Address")]
        public string Address { get; set; }
        [Required]
        public double Longtitude { get; set; }
        [Required]
        public double Latitude { get; set; }
        public RentalType RentalType { get; set; }

        public string? Area { get; set; }

        [Required(ErrorMessage = "Must Include AccountID")]
        public string AccountID { get; set; }
        public List<IFormFile> Images { get; set; }

    }
}
