using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.CultureExperiences
{
    public class GetAllCultureExperiencesResponse
    {
        [Required]
        public string CultureName { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? CultureType { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public bool Status { get; set; }
    }
}
