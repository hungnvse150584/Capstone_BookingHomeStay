using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Property
    {
        [Key]
        public int PropertyID { get; set; }

        [Required]
        public int numberBedRoom { get; set; }

        [Required]
        public int numberBathRoom { get; set; }

        [Required]
        public int numberWifi { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public ICollection<HomeStayTypes> HomeStayTypes { get; set; }

    }

