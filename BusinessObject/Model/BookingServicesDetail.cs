using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Model;

    public class BookingServicesDetail
    {
        [Key]
        public int BookingServicesDetailID { get; set; }

        public int Quantity { get; set; }

        public double unitPrice { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? RentHour { get; set; }

        public double TotalAmount { get; set; }

        public int? ServicesID { get; set; }
        [ForeignKey("ServicesID")]
        public Services? Services { get; set; }

        public int? BookingServicesID { get; set; }
        [ForeignKey("BookingServicesID")]
        public BookingServices? BookingService { get; set; }
    }

