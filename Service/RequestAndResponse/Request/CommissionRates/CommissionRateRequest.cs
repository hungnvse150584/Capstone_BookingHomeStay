using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.CommissionRates
{
    public class CommissionRateRequest
    {
        public ICollection<CommissionRateRequest> HomeStays { get; set; }
    }
}
