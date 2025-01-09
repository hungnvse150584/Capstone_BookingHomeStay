using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Provinces;
using Service.RequestAndResponse.Response.Streets;
using Service.RequestAndResponse.Response.Wards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Locations
{
    public class GetLocation
    {
        public int LocationID { get; set; }

        public string? numberHouse { get; set; }


        public string? postalCode { get; set; }


        public string? Cooordinate { get; set; }


        /*public int? StreetID { get; set; }*/
        public GetStreet Street { get; set; }

        /*public int? WardID { get; set; }*/
        public GetWard Ward { get; set; }


        /*public int? DistrictID { get; set; }*/
        public GetDistrict District { get; set; }


        /*public int? ProvinceID { get; set; }*/
        public GetProvince Province { get; set; }
    }
}
