using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class BookingDetail
    {
        [Key]
        public int BookingDetailID { get; set; }

        public double rentPrice { get; set; }

        public int Quantity { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public double TotalAmount { get; set; }

        public int? HomeStayTypesID { get; set; }
        [ForeignKey("HomeStayTypesID ")]
        public HomeStayTypes? HomeStayTypes { get; set; }

        public int? BookingID { get; set; }
        [ForeignKey("BookingID ")]
        public Booking? Booking { get; set; }
    }

