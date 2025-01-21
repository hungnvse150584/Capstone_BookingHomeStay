using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Request.BookingServiceDetails
{
    public class BookingServiceDetailRequest
    {
        public int Quantity { get; set; }

        public double unitPrice { get; set; }

        public double TotalAmount { get; set; }

        public int ServicesID { get; set; }
    }
}
