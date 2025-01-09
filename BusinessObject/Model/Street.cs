using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Street
    {
        [Key]
        public int StreetID { get; set; }

        [Required]
        public string streetName { get; set; }

        public int? WardID { get; set; }
        [ForeignKey("WardID ")]
        public Ward? Ward { get; set; }

        public ICollection<Location> Locations { get; set; }
    }

