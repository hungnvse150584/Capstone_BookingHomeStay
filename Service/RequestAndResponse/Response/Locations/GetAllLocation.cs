using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Locations
{
    public class GetAllLocation
    {
        public int LocationID { get; set; }

        public string? numberHouse { get; set; }

        
        public string? postalCode { get; set; }

        
        public string? Cooordinate { get; set; }

        
        public int? StreetID { get; set; }

        
        public int? WardID { get; set; }

        
        public int? DistrictID { get; set; }

        
        public int? ProvinceID { get; set; }
    }
}
