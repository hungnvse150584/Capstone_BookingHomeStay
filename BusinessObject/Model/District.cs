using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class District
    {
        [Key]
        public int DistrictID { get; set; }

        [Required]
        public string districtName { get; set; }

        public int? ProvinceID { get; set; }
        [ForeignKey("ProvinceID ")]
        public Province? Province { get; set; }

        public ICollection<Ward> Wards { get; set; }

        public ICollection<Location> Locations { get; set; }
    }

