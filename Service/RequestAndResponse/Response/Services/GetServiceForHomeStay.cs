using Service.RequestAndResponse.Response.ImageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Services
{
    public class GetServiceForHomeStay
    {
        public string servicesName { get; set; }

        public string Description { get; set; }
        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public double UnitPrice { get; set; }

        public double servicesPrice { get; set; }

        public bool Status { get; set; }
        public IEnumerable<GetAllImageService> ImageServices { get; set; }
    }
}
