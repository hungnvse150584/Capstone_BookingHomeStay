using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Streets;

namespace Service.RequestAndResponse.Response.Wards
{
    public class GetAllWard
    {
        
        public int WardID { get; set; }

        
        public string wardName { get; set; }

        public int? DistrictID { get; set; }
        
        public GetDistrict? District { get; set; }

        public ICollection<GetStreet> Streets { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}
