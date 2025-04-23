﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class ImageRating
    {
        [Key]
        public int ImageRatingID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? RatingID { get; set; }
        [ForeignKey("RatingID")]
        public Rating? Ratings { get; set; }
    }
}
