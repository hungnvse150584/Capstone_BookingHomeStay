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

        public double SumRate { get; set; }

        public double CleaningRate { get; set; }

        public double ServiceRate { get; set; }

        public double FacilityRate { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("AccountID")]
        public string AccountID { get; set; }
        public Account Account { get; set; }

        public int? HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }

        public ICollection<ImageRating> ImageRatings { get; set; }
    }

