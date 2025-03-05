using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class ImageHomeStayRentals
    {
        [Key]
        public int ImageHomeStayRentalsID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? HomeStayRentalID { get; set; }
        [ForeignKey("HomeStayRentalID")]
        public HomeStayRentals? HomeStayRentals { get; set; }
    }

