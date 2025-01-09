using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.Wards;

namespace Service.RequestAndResponse.Response.Streets
{
    public class GetAllStreet
    {
        public int StreetID { get; set; }

        public string streetName { get; set; }

        public int? WardID { get; set; }
        
        public GetWard? Ward { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}
