using BusinessObject.Model;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingOfServicesDetails
{
    public class GetAllDetailOfServices
    {
        public int BookingServicesDetailID { get; set; }

        public int Quantity { get; set; }

        public double unitPrice { get; set; }

        public double TotalAmount { get; set; }

        public int? ServicesID { get; set; }
        public GetAllServices? Services { get; set; }

        public int? BookingServicesID { get; set; }
        public GetAllBookingServices? BookingService { get; set; }
    }
}
