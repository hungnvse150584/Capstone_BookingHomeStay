using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Service.RequestAndResponse.Request.Services
{
    public class CreateServices
    {
        [Required]
        public string servicesName { get; set; }

        [Required]
        public string Description { get; set; }

        //public DateTime CreateAt { get; set; }

        //public DateTime UpdateAt { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public double servicesPrice { get; set; }

        public bool Status { get; set; }

        public int? HomeStayID { get; set; }
        [Required]
        public List<IFormFile> Images { get; set; }
    }
}
