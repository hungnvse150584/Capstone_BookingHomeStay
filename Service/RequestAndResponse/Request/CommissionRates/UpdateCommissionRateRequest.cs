using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.CommissionRates
{
    public class UpdateCommissionRateRequest
    {
        [Key]
        public int CommissionRateID { get; set; }

        public double HostShare { get; set; }

        public double PlatformShare { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public int HomeStayID { get; set; }
    }
}
