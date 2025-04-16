using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Services
{
    public class ServiceWithTotalPriceResponse
    {
        public int ServicesID { get; set; }

        public string servicesName { get; set; }

        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public double UnitPrice { get; set; }

        public double servicesPrice { get; set; }

        public double TotalPrice { get; set; } 

        public bool Status { get; set; }

        public int? HomeStayID { get; set; }

        public ServiceType ServiceType { get; set; }
    }
}
