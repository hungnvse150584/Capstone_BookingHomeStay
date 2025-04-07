using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Services
{
    public class GetServiceForAccount
    {
        public string servicesName { get; set; }

        public string Description { get; set; }

        public double UnitPrice { get; set; }

        public double servicesPrice { get; set; }

        public bool Status { get; set; }
    }
}
