using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Ward
{
    public class AddWardRequest
    {
        [Required]
        public string wardName { get; set; }

        [Required]
        public int? DistrictID { get; set; }
    }
}
