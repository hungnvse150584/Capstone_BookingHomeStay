using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Ward
    {
        [Key]
        public int WardID { get; set; }

        [Required]
        public string wardName { get; set; }

        public int? DistrictID { get; set; }
        [ForeignKey("DistrictID ")]
        public District? District { get; set; }

        public ICollection<Street> Streets { get; set; }

        public ICollection<Location> Locations { get; set; }
    }

