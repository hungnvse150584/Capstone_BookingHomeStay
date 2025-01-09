using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Location
    {
        [Key]
        public int LocationID { get; set; }

        [Required]
        public string? numberHouse { get; set; }

        [Required]
        public string? postalCode { get; set; }

        [Required]
        public string? Cooordinate { get; set; }

        public int? StreetID { get; set; }
        [ForeignKey("StreetID ")]
        public Street? Street { get; set; }

        public int? WardID { get; set; }
        [ForeignKey("WardID ")]
        public Ward? Ward { get; set; }

        public int? DistrictID { get; set; }
        [ForeignKey("DistrictID ")]
        public District? District { get; set; }

        public int? ProvinceID { get; set; }
        [ForeignKey("ProvinceID ")]
        public Province? Province { get; set; }

        public string FullAddress { get; set; }
    }

