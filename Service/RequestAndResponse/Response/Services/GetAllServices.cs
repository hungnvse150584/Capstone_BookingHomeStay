using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.BookingOfServicesDetails;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.ImageService;

namespace Service.RequestAndResponse.Response.Services
{
    public class GetAllServices
    {
        public int? ServicesID { get; set; }
        public string servicesName { get; set; }

        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public double UnitPrice { get; set; }

        public double servicesPrice { get; set; }

        public bool Status { get; set; }

        public int? HomeStayID { get; set; }
        //public HomeStayResponse? HomeStay { get; set; }
        public IEnumerable<GetAllImageService> ImageServices { get; set; }

        public ICollection<GetAllDetailOfServices> BookingServicesDetails { get; set; }
    }
}
