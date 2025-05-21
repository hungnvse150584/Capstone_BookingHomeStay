using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Services
{
    public class UpdateServices
    {
        //[Required]
        //public int servicesID { get; set; }

        [Required]
        public string servicesName { get; set; }

        [Required]
        public string Description { get; set; }



        [Required]
        public double servicesPrice { get; set; }

        public bool Status { get; set; }
        public int? Quantity {  get; set; }

        public int? HomeStayID { get; set; }

        //public List<IFormFile> Images { get; set; }

  
        public ServiceType? ServiceType { get; set; }
    }
}
