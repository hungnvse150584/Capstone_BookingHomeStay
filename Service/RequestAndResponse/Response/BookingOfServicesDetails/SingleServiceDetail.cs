﻿using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.BookingOfServicesDetails
{
    public class SingleServiceDetail
    {
        public int BookingServicesDetailID { get; set; }

        public int Quantity { get; set; }

        public double unitPrice { get; set; }

        public int? dayRent { get; set; }

        public double TotalAmount { get; set; }

        public int? ServicesID { get; set; }
        public GetSingleService? Services { get; set; }

        public int? BookingServicesID { get; set; }
    }
}
