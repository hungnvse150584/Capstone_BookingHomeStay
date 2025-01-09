using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Street
{
    public class AddStreetRequest
    {
        [Required]
        public string streetName { get; set; }

        [Required]
        public int? WardID { get; set; }
    }
}
