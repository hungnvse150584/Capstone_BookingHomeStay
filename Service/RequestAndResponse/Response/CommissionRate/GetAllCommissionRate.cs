using BusinessObject.Model;
using Service.RequestAndResponse.Response.HomeStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.CommissionRate
{
    public class GetAllCommissionRate
    {
        public int CommissionRateID { get; set; }

        public double HostShare { get; set; }

        public double PlatformShare { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public ICollection<HomeStayResponse> HomeStays { get; set; }
    }
}
