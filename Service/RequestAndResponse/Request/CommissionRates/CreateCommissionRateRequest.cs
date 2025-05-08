using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.CommissionRates
{
    public class CreateCommissionRateRequest
    {
        public double HostShare { get; set; }

        public double PlatformShare { get; set; }

        //public DateTime CreateAt { get; set; }

        public int HomeStayID { get; set; }
    }
}
