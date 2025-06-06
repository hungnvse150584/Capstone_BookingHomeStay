﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RequestAndResponse.Response.Accounts
{
    public class GetCustomerUser
    {
        public string AccountID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int TotalBooking { get; set; }
        public string? homestayName { get; set; }
    }
}
