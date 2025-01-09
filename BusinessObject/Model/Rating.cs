using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class Rating
    {
        [Key]
        public int RatingID { get; set; }

        public int Rate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID ")]
        public HomeStay? HomeStay { get; set; }
    }

