﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model
{
    public class Transaction
    {
        [Key]
        public string ResponseId { get; set; }
        //
        //public int OrderID { get; set; }
        //[ForeignKey("OrderID")]
        public Booking? Booking { get; set; } = null!;

        public BookingServices? BookingService { get; set; } = null!;

        public HomeStay? HomeStay { get; set; } = null!;

        public Account? Account { get; set; } = null!;

        public string TmnCode { get; set; }

        public string TxnRef { get; set; }

        public long Amount { get; set; }

        public string OrderInfo { get; set; }

        public string ResponseCode { get; set; }

        public string Message { get; set; }

        public string BankTranNo { get; set; }

        public DateTime PayDate { get; set; }
        public string BankCode { get; set; }

        public string TransactionNo { get; set; }

        public string TransactionType { get; set; }

        public string TransactionStatus { get; set; }

        public string SecureHash { get; set; }
    }
}
