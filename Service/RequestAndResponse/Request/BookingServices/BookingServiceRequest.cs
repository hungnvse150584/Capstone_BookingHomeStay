using BusinessObject.Model;
using Service.RequestAndResponse.Request.BookingDetail;
using Service.RequestAndResponse.Request.BookingServiceDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingServices
{
    public class BookingServiceRequest
    {

        public ICollection<BookingServiceDetailRequest> BookingServicesDetails { get; set; }

    }
}
