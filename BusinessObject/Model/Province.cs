using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Province
    {
        [Key]
        public int ProvinceID { get; set; }

        [Required]
        public string provinceName { get; set; }

        public ICollection<District> Districts { get; set; }

        public ICollection<Location> Locations { get; set; }
    }

