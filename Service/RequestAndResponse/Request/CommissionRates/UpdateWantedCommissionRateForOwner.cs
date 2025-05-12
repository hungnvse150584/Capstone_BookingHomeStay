using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.CommissionRates
{
    public class UpdateWantedCommissionRateForOwner
    {
        [Key]
        public int CommissionRateID { get; set; }
        public double? WantedHostShare { get; set; }
        public bool? ownerAccepted { get; set; }
    }
}
