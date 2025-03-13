using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class ImageCultureExperience
    {
        [Key]
        public int ImageCultureExperiencesID { get; set; }

        [Required]
        public string? Image { get; set; }

        public int? CultureExperienceID { get; set; }
        [ForeignKey("CultureExperienceID")]
        public CultureExperience? CultureExperiences { get; set; }
    }
}
