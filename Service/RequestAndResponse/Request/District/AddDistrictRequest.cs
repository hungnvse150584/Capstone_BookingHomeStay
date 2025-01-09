using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.District
{
    public class AddDistrictRequest
    {
        [Required]
        public string districtName { get; set; }

        [Required]
        public int? ProvinceID { get; set; }
        
    }
}
