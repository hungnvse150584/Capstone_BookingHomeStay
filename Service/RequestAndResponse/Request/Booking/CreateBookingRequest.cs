using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Request.BookingDetail;
using Service.RequestAndResponse.Request.BookingServices;

namespace Service.RequestAndResponse.Request.Booking
{
    public class CreateBookingRequest
    {
        public int numberOfChildren { get; set; }

        public int numberOfAdults { get; set; }

        public string AccountID { get; set; }

        public int HomeStayID { get; set; }

        public ICollection<BookingDetailRequest>  BookingDetails { get; set; }
        public BookingServiceRequest? BookingOfServices { get; set; }
    }
}
