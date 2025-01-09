using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.Provinces;
using Service.RequestAndResponse.Response.Wards;

namespace Service.RequestAndResponse.Response.Districts
{
    public class GetAllDistrict
    {
        
        public int DistrictID { get; set; }

        
        public string districtName { get; set; }

        public int? ProvinceID { get; set; }
        
        public GetProvince? Province { get; set; }

        public ICollection<GetWard> Wards { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}
