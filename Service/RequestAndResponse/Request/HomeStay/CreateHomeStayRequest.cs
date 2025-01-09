using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Request.Location;

namespace Service.RequestAndResponse.Request.HomeStay
{
    public class CreateHomeStayRequest
    {
        [Required(ErrorMessage = "HomeStay must have name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "HomeStay must have Description")]
        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

       
        public HomeStayStatus Status { get; set; }

        public string Area { get; set; }

        [Required(ErrorMessage = "Must Include AccountID")]
        public string AccountID { get; set; }


        [Required(ErrorMessage = "Must Include Location")]
        public LocationRequest Location { get; set; }
    }
}
