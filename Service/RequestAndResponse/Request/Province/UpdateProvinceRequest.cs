using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.Province
{
    public class UpdateProvinceRequest
    {
        [Required]
        public string provinceName { get; set; }
    }
}
