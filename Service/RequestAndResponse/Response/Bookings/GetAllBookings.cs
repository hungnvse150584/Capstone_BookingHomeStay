﻿using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Response.BookingDetails;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.Reports;

namespace Service.RequestAndResponse.Response.Bookings
{
    public class GetAllBookings
    {
        public int BookingID { get; set; }

        public DateTime BookingDate { get; set; }

        public int numberOfChildren { get; set; }

        public int numberOfAdults { get; set; }

        public BookingStatus Status { get; set; }

        public double TotalRentPrice { get; set; }

        public double Total { get; set; }

        public double bookingDeposit { get; set; }

        public double remainingBalance { get; set; }

        public int? ReportID { get; set; }
        public GetReportResponse? Report { get; set; }

        public string AccountID { get; set; }
        public GetAccountUser Account { get; set; }


        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<GetBookingDetails> BookingDetails { get; set; }

        public ICollection<GetSimpleBookingService> BookingServices { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
