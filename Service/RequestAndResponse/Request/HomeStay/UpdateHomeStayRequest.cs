using BusinessObject.Model;
using Service.RequestAndResponse.Request.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.HomeStay
{
    public class UpdateHomeStayRequest
    {
        [Required(ErrorMessage = "HomeStay must have name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "HomeStay must have Description")]
        public string Description { get; set; }

        public DateTime UpdateAt { get; set; }

        public HomeStayStatus Status { get; set; }

        public RentalType RentalType { get; set; }

        public string Area { get; set; }

        [Required(ErrorMessage = "Must Include AccountID")]
        public string AccountID { get; set; }


        [Required(ErrorMessage = "Must Include Location")]
        public UpdateLocationRequest Location { get; set; }
    }
}
