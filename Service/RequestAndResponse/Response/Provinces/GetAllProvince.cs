using BusinessObject.Model;
using Service.RequestAndResponse.Response.Districts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Provinces
{
    public class GetAllProvince
    {
        
        public int ProvinceID { get; set; }

        public string provinceName { get; set; }

        public ICollection<GetDistrict> Districts { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}
