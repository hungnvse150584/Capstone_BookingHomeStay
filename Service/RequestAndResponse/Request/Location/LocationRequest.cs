using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Location
{
    public class LocationRequest
    {
        [Required(ErrorMessage = "NumberHouse is required.")]
        public string? numberHouse { get; set; }

        [Required(ErrorMessage = "PostalCode is required.")]
        public string? postalCode { get; set; }

        [Required(ErrorMessage = "Coordinate is required.")]
        public string? Cooordinate { get; set; }

        [Required(ErrorMessage = "StreetID is required.")]
        public int? StreetID { get; set; }

        [Required(ErrorMessage = "WardID is required.")]
        public int? WardID { get; set; }

        [Required(ErrorMessage = "DistrictID is required.")]
        public int? DistrictID { get; set; }

        [Required(ErrorMessage = "ProvinceID is required.")]
        public int? ProvinceID { get; set; }
        
    }
}
